using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // WAJIB: Seret objek Kurir ke slot ini di Inspector
    public Transform target; 
    
    // Kecepatan kamera mengejar (biasanya 0.1 atau lebih rendah untuk efek halus)
    public float smoothSpeed = 0.125f; 
    
    // Offset (Jarak) kamera dari target (misalnya, agar kamera sedikit di atas karakter)
    public Vector3 offset; 

    // Update dipanggil setelah semua Update() lainnya (agar pergerakan karakter selesai dulu)
    void LateUpdate()
    {
        if (target == null)
        {
            // Jika target (Kurir) tidak ditemukan, jangan lakukan apa-apa.
            return;
        }

        // Posisi yang diinginkan kamera (Posisi Kurir + Offset)
        Vector3 desiredPosition = target.position + offset;
        
        // Interpolsi linier (SmoothDamp) untuk pergerakan yang mulus.
        // Gunakan transform.position (posisi kamera) sebagai posisi saat ini
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Terapkan posisi yang sudah diperhalus ke kamera
        transform.position = smoothedPosition;
    }
}