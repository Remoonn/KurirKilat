using UnityEngine;

public class PickupPoint : MonoBehaviour
{
    [Header("Settings Misi")]
    public int targetDeliveryID = -1;
    public GameObject missionIndicator;

    private bool isMissionActive = false;

    public void SetActiveMission(bool isActive)
    {
        isMissionActive = isActive;
        if (missionIndicator != null)
            missionIndicator.SetActive(isActive);
    }

    public bool IsCurrentMissionActive()
    {
        return isMissionActive;
    }

    public void AcceptMission()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        MissionManager manager = FindObjectOfType<MissionManager>();

        if (player == null || manager == null || targetDeliveryID == -1)
            return;

        if (player.activeDeliveryIDs.Contains(targetDeliveryID))
            return;

        // Tambahkan paket
        player.activeDeliveryIDs.Add(targetDeliveryID);

        // INIT durability SEKALI
        if (!player.packageDurabilityRegistry.ContainsKey(targetDeliveryID))
        {
            player.packageDurabilityRegistry.Add(
                targetDeliveryID,
                player.maxDurability
            );
        }
        // if (!player.packageTimerRegistry.ContainsKey(targetDeliveryID))
        // {
        //     player.packageTimerRegistry.Add(targetDeliveryID, player.defaultMaxTime);
        // }
        else
        {
            // Jika paket baru benar-benar pertama kali diambil
            player.packageTimerRegistry.Add(targetDeliveryID, player.defaultMaxTime);
        }

        // 🔴 PAKET BARU WAJIB AKTIF
       //player.ForceSelectPackage(targetDeliveryID);

        // Update marker & misi berikutnya
        manager.UpdateDestinationMarker(targetDeliveryID);
        manager.GenerateNewMission();

        // Update UI inventory
        PlayerMovement.OnInventoryChanged?.Invoke();

        SetActiveMission(false);

        Debug.Log($"[MISI DITERIMA] Paket ID {targetDeliveryID}");
    }
}
