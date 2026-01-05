using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    // WAJIB DITAMBAHKAN AGAR MUNCUL DI INSPECTOR
    public int missionID; 

    public int rewardAmount = 50; // (Reward dari panduan sebelumnya)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ... (Logika verifikasi misi akan ada di sini)
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
