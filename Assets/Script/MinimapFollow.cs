// MinimapFollow.cs
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    // WAJIB: Seret objek Kurir (Player) ke slot ini di Inspector
    public Transform target; 

    // Gunakan LateUpdate agar kamera bergerak setelah pemain bergerak
    void LateUpdate()
    {
        if (target == null) return;

        // Ambil posisi target (Kurir)
        Vector3 targetPosition = target.position;

        // Buat posisi baru untuk kamera.
        // Kita hanya mengambil X dan Y dari target, dan mempertahankan Z kamera saat ini.
        // Mempertahankan Z kamera sangat penting agar kamera tetap berada di atas (Z=-10).
        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        // Terapkan posisi baru ke kamera
        transform.position = newPosition;
    }
}