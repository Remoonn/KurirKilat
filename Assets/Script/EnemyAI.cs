using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float roamRange = 5f;
    public float waitTime = 2f;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRadius = 5.5f;
    public float loseTargetRadius = 8.0f;
    public GameObject visionConeObject;
    public LayerMask obstacleLayer; 

    [Header("Dialogue Profile")]
    public Sprite enemyPortrait; // Masukkan foto profil (Laki/Perempuan) di sini

    // ---------------- [TAMBAHAN MUSIK 1: Variabel] ----------------
    private GameMusicController musicController;
    // --------------------------------------------------------------

    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Animator anim;
    private Rigidbody2D rb;
    private bool isChasing = false;
    private bool isWaiting = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        if (player == null)
        {
            // Cek agar aman jika tidak menemukan player
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // ---------------- [TAMBAHAN MUSIK 2: Cari Script DJ] ----------------
        musicController = FindObjectOfType<GameMusicController>();
        // ---------------------------------------------------------------------

        SetNewTarget(); 
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (player == null) return; // Mencegah error jika player belum assign

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // ---------------- [TAMBAHAN MUSIK 3: Logika Pemicu] ----------------
        // Saya ubah sedikit jadi pakai kurung kurawal {} agar bisa muat kode musiknya
        
        if (distanceToPlayer <= detectionRadius) 
        {
            isChasing = true;
            // Panggil Musik Panik
            if (musicController != null) musicController.SetStatusDikejar(true);
        }
        else if (distanceToPlayer > loseTargetRadius) 
        {
            isChasing = false;
            // Panggil Musik Santai
            if (musicController != null) musicController.SetStatusDikejar(false);
        }
        // ---------------------------------------------------------------------

        if (isChasing) 
        {
            isWaiting = false; 
            MoveLogic();
        }
        else if (!isWaiting) 
        {
            MoveLogic();
        }
    }

    void MoveLogic()
    {
        float currentSpeed = isChasing ? chaseSpeed : patrolSpeed;
        Vector2 currentTarget = isChasing ? (Vector2)player.position : targetPosition;
        Vector2 moveDir = (currentTarget - (Vector2)transform.position).normalized;

        if (isChasing)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDir, 1.5f, obstacleLayer);
            if (hit.collider != null)
            {
                Vector2 leftDir = new Vector2(-moveDir.y, moveDir.x);
                Vector2 rightDir = new Vector2(moveDir.y, -moveDir.x);
                RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, leftDir, 1.5f, obstacleLayer);
                moveDir = (hitLeft.collider == null) ? leftDir : rightDir;
            }
        }

        rb.linearVelocity = moveDir * currentSpeed;

        if (moveDir.magnitude > 0.05f)
        {
            anim.SetFloat("MoveX", moveDir.x);
            anim.SetFloat("MoveY", moveDir.y);
            anim.SetFloat("Speed", currentSpeed);
            
            if (visionConeObject != null)
            {
                float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                visionConeObject.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
        }

        if (!isChasing && Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            StartCoroutine(WaitAtPoint());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            isWaiting = false; 
            StopAllCoroutines(); 
            
            // ---------------- [TAMBAHAN MUSIK 4: Reset saat ketangkap] ----------------
            if (musicController != null) musicController.SetStatusDikejar(false);
            // --------------------------------------------------------------------------

            // Panggil DialogueManager dan kirimkan foto profil musuh ini
            // (Pakai FindObjectOfType agar lebih aman mencari scriptnya)
            var dialogueMgr = FindObjectOfType<DialogueManager>();
            if(dialogueMgr != null) dialogueMgr.OpenCatchDialogue(enemyPortrait);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isChasing && !collision.gameObject.CompareTag("Player"))
        {
            if (isWaiting) 
            {
                StopAllCoroutines();
                isWaiting = false; 
            }
            SetNewTarget(); 
        }
    }

    public void ResetToStartPosition()
    {
        StopAllCoroutines();
        isChasing = false;

        // ---------------- [TAMBAHAN MUSIK 5: Reset saat game ulang] ----------------
        if (musicController != null) musicController.SetStatusDikejar(false);
        // --------------------------------------------------------------------------

        SetNewTarget();
    }

    void SetNewTarget() 
    { 
        targetPosition = startPosition + new Vector2(Random.Range(-roamRange, roamRange), Random.Range(-roamRange, roamRange)); 
    }

    IEnumerator WaitAtPoint()
    {
        isWaiting = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);
        SetNewTarget();
        isWaiting = false;
    }
}