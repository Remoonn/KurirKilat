using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    // ================= EVENT =================
    public static System.Action OnInventoryChanged;

    // ================= MOVEMENT =================
    private Animator animator;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    private bool isStunned = false;

    // ================= PACKAGE SYSTEM =================
    [Header("Paket & Misi")]
    public List<int> activeDeliveryIDs = new List<int>();

    [Header("Durability Paket")]
    public float maxDurability = 100f;
    public Dictionary<int, float> packageDurabilityRegistry = new Dictionary<int, float>();

    [Header("UI")]
    public Slider durabilitySlider;

    private int currentlySelectedPackageID = -1;

    private float lastShownDurability = -1f;

    [Header("Timer System")]
    public Dictionary<int, float> packageTimerRegistry = new Dictionary<int, float>();
    public float defaultMaxTime = 60f; // Waktu default 60 detik per paket

    public int GetCurrentlySelectedPackageID()
{
    return currentlySelectedPackageID;
}

    // ================= ANIMATION =================
    private const int DIR_FRONT = 0;
    private const int DIR_RIGHT = 1;
    private const int DIR_LEFT = 2;
    private const int DIR_BACK = 3;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (durabilitySlider != null)
            durabilitySlider.gameObject.SetActive(false);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0;
    }

    void Update()
    {
        if (Time.timeScale == 0 || isStunned) return;
        Move();
        UpdateTimers();
    }

    // ==================================================
    // =============== PACKAGE CORE =====================
    // ==================================================

    /// <summary>
    /// DIPANGGIL GAMEPLAY (AMBIL MISI BARU)
    /// </summary>
    /// 
    void UpdateTimers()
{
    if (isStunned || Time.timeScale == 0) return;

    // Gunakan List sementara untuk menghindari error saat koleksi berubah
    List<int> keys = new List<int>(packageTimerRegistry.Keys); 
    foreach (int id in keys)
    {
        if (packageTimerRegistry[id] > 0)
        {
            packageTimerRegistry[id] -= Time.deltaTime;
            
            // JIKA WAKTU HABIS
            if (packageTimerRegistry[id] <= 0)
            {
                packageTimerRegistry[id] = 0;
                
                // Panggil GameOverManager untuk mengakhiri game
                GameOverManager goManager = FindObjectOfType<GameOverManager>();
                if (goManager != null)
                {
                    // Gunakan fungsi public TriggerGameOver (lihat poin berikutnya)
                    goManager.TriggerGameOverExternal("Waktu pengantaran paket telah habis!"); 
                }
            }
        }
    }
}

    

    void TriggerGameOver(string reason)
    {
        Debug.Log(reason);
        FindObjectOfType<GameOverManager>()?.CheckGameOverCondition(); 
        // Atau panggil panel game over langsung jika CheckGameOverCondition menangani kekalahan
    }
    public void ForceSelectPackage(int missionID)
    {
        if (!packageDurabilityRegistry.ContainsKey(missionID))
            return;

        currentlySelectedPackageID = missionID;

        
        UpdateDurabilityUI();
    }

    /// <summary>
    /// DIPANGGIL UI (KLIK ICON)
    /// </summary>
    public void SelectPackageToView(int missionID)
    {
        if (!packageDurabilityRegistry.ContainsKey(missionID))
            return;

        currentlySelectedPackageID = missionID;
        StartPackageTimer(missionID);

        UpdateDurabilityUI();
    }

    private void StartPackageTimer(int missionID)
    {
        // Hanya buat timer jika belum ada (mencegah reset waktu setiap kali diklik)
        if (!packageTimerRegistry.ContainsKey(missionID))
        {
            packageTimerRegistry.Add(missionID, defaultMaxTime);
            Debug.Log($"Timer untuk paket {missionID} dimulai!");
        }
    }

    public float GetPackageDurabilityPercent(int missionID)
    {
        if (!packageDurabilityRegistry.ContainsKey(missionID))
            return 0f;

        return packageDurabilityRegistry[missionID] / maxDurability;
    }
    public void TakePackageDamage(float damage)
    {
        if (currentlySelectedPackageID == -1) return;
        if (!packageDurabilityRegistry.ContainsKey(currentlySelectedPackageID)) return;

        packageDurabilityRegistry[currentlySelectedPackageID] -= damage;
        packageDurabilityRegistry[currentlySelectedPackageID] =
            Mathf.Clamp(packageDurabilityRegistry[currentlySelectedPackageID], 0, maxDurability);

        // Paket hancur
        if (packageDurabilityRegistry[currentlySelectedPackageID] <= 0)
        {
            int destroyedID = currentlySelectedPackageID;

            // 1. Hapus timer agar tidak Game Over di latar belakang (PENTING)
            if (packageTimerRegistry.ContainsKey(destroyedID))
            {
                packageTimerRegistry.Remove(destroyedID);
            }

            activeDeliveryIDs.Remove(destroyedID);
            RemovePackageData(destroyedID);

            OnInventoryChanged?.Invoke();
        }

        UpdateDurabilityUI();
    }

    public void RemovePackageData(int missionID)
    {
        if (packageDurabilityRegistry.ContainsKey(missionID))
            packageDurabilityRegistry.Remove(missionID);

        if (currentlySelectedPackageID == missionID)
        {
            currentlySelectedPackageID = -1;
            if (durabilitySlider != null)
                durabilitySlider.gameObject.SetActive(false);
        }
    }

    private void UpdateDurabilityUI()
    {
         if (durabilitySlider == null) return;

    if (currentlySelectedPackageID == -1 ||
        !packageDurabilityRegistry.ContainsKey(currentlySelectedPackageID))
    {
        durabilitySlider.gameObject.SetActive(false);
        lastShownDurability = -1f;
        return;
    }

    float currentValue = packageDurabilityRegistry[currentlySelectedPackageID];

    durabilitySlider.gameObject.SetActive(true);

    // ❗ SET SEKALI SAJA
    if (durabilitySlider.maxValue != maxDurability)
        durabilitySlider.maxValue = maxDurability;

    // 🔥 JANGAN UPDATE JIKA NILAI SAMA
    if (Mathf.Approximately(lastShownDurability, currentValue))
        return;

    lastShownDurability = currentValue;

    // 🔥 PAKAI INI AGAR TIDAK ADA EVENT / TRANSITION
    durabilitySlider.SetValueWithoutNotify(currentValue);
    }

    // ==================================================
    // =============== STATUS ============================
    // ==================================================

    public void StopMoving()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (animator != null)
        {
            animator.SetBool("IsIdle", true);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
        }
    }

    public void ApplyStunEffect(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        float originalWalk = walkSpeed;
        float originalRun = runSpeed;

        walkSpeed /= 2;
        runSpeed /= 2;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(0.6f, 0.6f, 0.6f);

        yield return new WaitForSecondsRealtime(duration);

        walkSpeed = originalWalk;
        runSpeed = originalRun;

        if (sr != null) sr.color = Color.white;
        isStunned = false;
    }

    // ==================================================
    // =============== MOVEMENT ==========================
    // ==================================================

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 input = new Vector2(x, y).normalized;
        bool isMoving = input.magnitude > 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = isRunning ? runSpeed : walkSpeed;

        if (isMoving)
        {
            transform.position += (Vector3)input * speed * Time.deltaTime;
            SetDirection(input);
        }

        SetAnimation(isMoving, isRunning);
    }

    private void SetAnimation(bool isMoving, bool isRunning)
    {
        if (animator == null) return;

        animator.SetBool("IsIdle", !isMoving);
        animator.SetBool("IsWalking", isMoving && !isRunning);
        animator.SetBool("IsRunning", isMoving && isRunning);
    }

    private void SetDirection(Vector2 input)
    {
        if (animator == null) return;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            animator.SetInteger("Direction", input.x > 0 ? DIR_RIGHT : DIR_LEFT);
        else
            animator.SetInteger("Direction", input.y > 0 ? DIR_BACK : DIR_FRONT);
    }
}
