using UnityEngine;
using System.Linq;

public class DeliveryZone : MonoBehaviour
{
    [Header("Settings")]
    public int missionID;
    public int rewardAmount = 100;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        MissionManager manager = FindObjectOfType<MissionManager>();

        if (player == null || manager == null)
            return;

        // Pastikan player membawa paket ini
        if (!player.activeDeliveryIDs.Contains(missionID))
            return;

        // Pastikan ini target aktif
        if (manager.activeDeliveryTarget != this)
            return;

        // ===============================
        // HITUNG REWARD BERDASARKAN KONDISI
        // ===============================

        float durabilityPercent = player.GetPackageDurabilityPercent(missionID);

        // Jika paket hancur total → tidak dibayar
        if (durabilityPercent <= 0f)
        {
            Debug.Log($"[DELIVERY GAGAL] Paket {missionID} hancur total.");
        }
        else
        {
            int finalReward = Mathf.RoundToInt(rewardAmount * durabilityPercent);
            Wallet.AddMoney(finalReward);

            Debug.Log(
                $"[DELIVERY SUKSES] Paket {missionID}\n" +
                $"Durability: {Mathf.RoundToInt(durabilityPercent * 100)}%\n" +
                $"Reward: Rp{finalReward}"
            );
        }

        // ===============================
        // BERSIHKAN DATA PAKET
        // ===============================

        player.activeDeliveryIDs.Remove(missionID);
        player.RemovePackageData(missionID);

        // TAMBAHKAN BARIS INI:
    if (player.packageTimerRegistry.ContainsKey(missionID))
    {
        player.packageTimerRegistry.Remove(missionID);
    }

        // 3. Tandai player sudah pernah sukses
        FindObjectOfType<GameOverManager>()
            ?.NotifyDeliveryCompleted();

        // 4. Cek game over (AMAN)
        FindObjectOfType<GameOverManager>()
            ?.CheckGameOverCondition();

        player.packageTimerRegistry.Remove(missionID);

        // Update UI inventory
        PlayerMovement.OnInventoryChanged?.Invoke();

        // ===============================
        // UPDATE MARKER & MISI
        // ===============================

        if (player.activeDeliveryIDs.Count > 0)
        {
            manager.UpdateDestinationMarker(player.activeDeliveryIDs[0]);
        }
        else
        {
            manager.UpdateDestinationMarker(-1);
            manager.GenerateNewMission();
        }
    }
}
