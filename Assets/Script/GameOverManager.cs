using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    private PlayerMovement player;

    [Header("UI Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text reasonText;

    private bool hasEverCompletedDelivery = false;
    private bool isGameOver = false;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    // DIPANGGIL SAAT PAKET BERHASIL DIANTAR
    public void NotifyDeliveryCompleted()
    {
        hasEverCompletedDelivery = true;
        Debug.Log("FLAG hasEverCompletedDelivery = TRUE");
        CheckGameOverCondition();
    }

    // DIPANGGIL SAAT STATE BERUBAH
    public void CheckGameOverCondition()
    {
        if (isGameOver) return;
        if (!hasEverCompletedDelivery) return;
        if (player == null) return;

        bool noMoney = Wallet.currentMoney <= 0;
        bool noPackage = player.activeDeliveryIDs.Count == 0;

        if (noMoney && noPackage)
        {
            TriggerGameOver("Kamu kehabisan modal untuk melanjutkan pengantaran.");
        }
        Debug.Log("CHECK GAME OVER");
    }

    private void TriggerGameOver(string reason)
    {
        isGameOver = true;

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (reasonText != null)
            reasonText.text = reason;

        Debug.Log("GAME OVER");
    }

    // DIPANGGIL OLEH BUTTON
    public void RetryGame()
    {
        // 1. Reset data statis (Wallet) agar uang kembali ke nol atau modal awal
        Wallet.currentMoney = 0; // Sesuaikan jika modal awal bukan 0
        
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void KeluarKeMainMenu()
    {
        Time.timeScale = 1f; // PENTING: Kembalikan waktu ke normal sebelum pindah scene
        SceneManager.LoadScene("MainMenu"); // Pastikan nama scene sesuai di folder Assets
    }
    
    public void TriggerGameOverExternal(string reason)
    {
        // Cek agar tidak memicu game over berkali-kali
        if (isGameOver) return;
        
        TriggerGameOver(reason);
    }
}
