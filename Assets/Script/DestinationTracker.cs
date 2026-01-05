using UnityEngine;

public class DestinationTracker : MonoBehaviour
{
    private Transform targetDestination;

    [Header("Flexibility Settings")]
    public float rotationSpeed = 5f;       // Kecepatan putaran (halus)
    public float floatingAmplitude = 0.1f; // Jarak melayang naik-turun
    public float floatingSpeed = 2f;       // Kecepatan melayang

    private Vector3 initialLocalPos;

    void Start()
    {
        initialLocalPos = transform.localPosition; // Simpan posisi relatif terhadap player
    }

    public void SetTarget(Transform newTarget)
    {
        targetDestination = newTarget;
        gameObject.SetActive(true); 
    }

    public void DisableTracker()
    {
        targetDestination = null;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (targetDestination == null)
        {
            gameObject.SetActive(false);
            return; 
        }

        // 1. ROTASI HALUS (Agar tidak kaku)
        Vector3 direction = targetDestination.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Sesuaikan -90f jika sprite panah Anda menghadap ke atas
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        
        // Lerp membuat transisi rotasi mengikuti target secara smooth
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // 2. EFEK MELAYANG (Bobbing Effect)
        float newY = initialLocalPos.y + Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude;
        transform.localPosition = new Vector3(initialLocalPos.x, newY, initialLocalPos.z);
    }
}