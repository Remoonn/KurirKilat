// MissionManager.cs

using UnityEngine;
using System.Linq; 
using System.Collections.Generic; // WAJIB: Tambahkan ini jika belum ada

public class MissionManager : MonoBehaviour
{
    // WAJIB DIISI DI INSPECTOR: Seret objek DestinationMarker ke slot ini
    public GameObject destinationMarkerPrefab;
    public DestinationTracker arrowTracker;

    // VARIABEL: Menyimpan posisi tujuan yang aktif
    [HideInInspector]
    public Vector3 activeMarkerPosition = Vector3.zero;
    
    // VARIABEL PENTING: Menyimpan objek DeliveryZone yang SEBENARNYA dituju.
    [HideInInspector]
    public DeliveryZone activeDeliveryTarget; 

    private PickupPoint[] allPickupPoints;
    private DeliveryZone[] allDeliveryZones;
    private DeliveryZone lastTarget;
    

    void Start()
    {
        allPickupPoints = FindObjectsOfType<PickupPoint>();
        allDeliveryZones = FindObjectsOfType<DeliveryZone>();

        Debug.Log($"MissionManager Aktif. Pickup: {allPickupPoints.Length}, Delivery: {allDeliveryZones.Length}");

        if (allPickupPoints.Length > 0 && allDeliveryZones.Length > 0)
        {
            GenerateNewMission();
        }
        
        // Pastikan penanda marker nonaktif saat startup
        if (destinationMarkerPrefab != null)
        {
            destinationMarkerPrefab.SetActive(false);
        }
    }

   public void GenerateNewMission()
    {
        foreach (var point in allPickupPoints)
        {
           if (point != null) point.SetActiveMission(false); 
        }

        if (allPickupPoints.Length == 0 || allDeliveryZones.Length == 0) return;

        PickupPoint randomPickup = allPickupPoints[Random.Range(0, allPickupPoints.Length)];
        int requiredID = randomPickup.targetDeliveryID; 
        
        DeliveryZone[] validDeliveryZones = allDeliveryZones
            .Where(dz => dz.missionID == requiredID)
            .ToArray();

        if (validDeliveryZones.Length == 0) return; 
        
        // --- PERUBAHAN 2: Logika Pemilihan Acak yang Lebih Variatif ---
        DeliveryZone randomDelivery;

        // Jika ada lebih dari satu pilihan peti, lakukan loop agar tidak memilih peti yang sama dengan sebelumnya
        if (validDeliveryZones.Length > 1)
        {
            do {
                randomDelivery = validDeliveryZones[Random.Range(0, validDeliveryZones.Length)];
            } while (randomDelivery == lastTarget);
        }
        else
        {
            randomDelivery = validDeliveryZones[0];
        }

        // Simpan peti ini sebagai 'peti terakhir' untuk pengecekan misi berikutnya
        lastTarget = randomDelivery; 
        // -------------------------------------------------------------

        Vector3 targetPos = randomDelivery.transform.position;
        activeMarkerPosition = new Vector3(targetPos.x, targetPos.y + 0.8f, targetPos.z); 

        randomPickup.SetActiveMission(true); 
        randomPickup.gameObject.SetActive(true); 

        Debug.Log($"Misi Baru: Antar ke {randomDelivery.gameObject.name}");
    }
    // FUNGSI BARU: Dipanggil oleh PickupPoint/UI untuk mengatur marker
    public void UpdateDestinationMarker(int packageID)
    {
        DeliveryZone targetZone = allDeliveryZones.FirstOrDefault(dz => dz.missionID == packageID);

    if (targetZone != null)
    {
        // --- BARIS PENTING ---
        // Pastikan target ini didaftarkan sebagai target aktif agar DeliveryZone mau menerima paket
        activeDeliveryTarget = targetZone; 

        destinationMarkerPrefab.transform.position = targetZone.transform.position + Vector3.up * 0.8f;
        destinationMarkerPrefab.SetActive(true);

        if (arrowTracker != null)
        {
            arrowTracker.SetTarget(targetZone.transform);
        }
    }
    else
    {
        destinationMarkerPrefab.SetActive(false);
        if (arrowTracker != null) arrowTracker.DisableTracker();
    }
    }
}