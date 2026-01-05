using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1.5f; 
    public float roamRange = 5f;   
    public float waitTime = 2f;    
    public LayerMask obstacleLayer; // Masukkan Layer "Obstacle" atau "Wall" di sini

    [Header("Detection Settings")]
    public float detectionDistance = 0.5f; // Seberapa jauh NPC melihat ke depan

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isWaiting = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position; 

        if (rb != null) rb.gravityScale = 0; 

        SetNewTarget(); 
    }

    void Update()
    {
        if (Time.timeScale == 0) 
        {
            rb.linearVelocity = Vector2.zero;
            if (anim != null) anim.SetFloat("Speed", 0);
            return;
        }

        if (!isWaiting) 
        {
            MoveLogic();
        }
    }

    void MoveLogic()
    {
        Vector2 currentPos = transform.position;
        Vector2 direction = (targetPosition - currentPos);
        
        // Cek jika sudah sampai target
        if (direction.magnitude < 0.2f)
        {
            StartCoroutine(WaitAtPoint());
            return;
        }

        Vector2 moveDir = direction.normalized;

        // --- SISTEM PENGHINDARAN (AVOIDANCE) ---
        // Tembakkan garis (Raycast) ke arah gerak
        RaycastHit2D hit = Physics2D.Raycast(currentPos, moveDir, detectionDistance, obstacleLayer);
        
        // Jika mendeteksi halangan
        if (hit.collider != null)
        {
            // Belok 90 derajat secara manual untuk mencari jalan keluar
            moveDir = Vector2.Perpendicular(moveDir); 
        }

        rb.linearVelocity = moveDir * walkSpeed;
        UpdateAnimation(moveDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika benar-benar mentok (Physical Collision), cari target baru di area lain
        if (!collision.gameObject.CompareTag("Player"))
        {
            StopAllCoroutines();
            SetNewTarget();
        }
    }

    void UpdateAnimation(Vector2 moveDir)
    {
        if (moveDir.magnitude > 0.05f)
        {
            float snapX = 0f;
            float snapY = 0f;

            if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.y))
                snapX = moveDir.x > 0 ? 1f : -1f;
            else
                snapY = moveDir.y > 0 ? 1f : -1f;

            anim.SetFloat("MoveX", snapX);
            anim.SetFloat("MoveY", snapY);
            anim.SetFloat("Speed", walkSpeed);
        }
    }

    void SetNewTarget() 
    { 
        // Mencari target acak di sekitar posisi awal
        targetPosition = startPosition + new Vector2(Random.Range(-roamRange, roamRange), Random.Range(-roamRange, roamRange)); 
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.SetFloat("Speed", 0); 
        yield return new WaitForSeconds(waitTime);
        SetNewTarget();
        isWaiting = false;
    }

    // Untuk visualisasi di Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 dir = (Vector3)targetPosition - transform.position;
        Gizmos.DrawRay(transform.position, dir.normalized * detectionDistance);
    }
}