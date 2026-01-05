using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; // Wajib untuk Coroutine

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI contentText;
    public Button bribeButton; 
    public Image enemyFaceImageDisplay; // Slot Image di UI untuk wajah musuh
    
    [Header("Settings")]
    public int bribeCost = 100;
    
    private int caughtWithoutItemsCount = 0; 
    private bool isDialogueActive = false;

    void Start() 
    { 
        if(dialoguePanel != null) dialoguePanel.SetActive(false); 
    }

    // Fungsi sekarang menerima parameter Sprite agar gambar wajah dinamis
    public void OpenCatchDialogue(Sprite face)
    {
        if (isDialogueActive) return;
        
        // 1. Hentikan pergerakan karakter agar tidak terseret
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.StopMoving(); 
        }

        // 2. Aktifkan Panel
        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        Time.timeScale = 0f; // Pause game

        // 3. Tampilkan wajah musuh yang menangkap
        if (enemyFaceImageDisplay != null && face != null)
        {
            enemyFaceImageDisplay.sprite = face;
        }
        
        // 4. Atur Interaksi Tombol Sogok
        if (bribeButton != null)
        {
            bribeButton.interactable = (Wallet.currentMoney >= bribeCost);
        }

        // 5. Tentukan Teks Dialog
        if (Wallet.currentMoney < bribeCost && (player == null || player.activeDeliveryIDs.Count == 0))
            contentText.text = "Woi! Mau lewat mana lu? Gak punya duit ya?";
        else
            contentText.text = "Woi! Bayar Rp " + bribeCost + " atau paket lu gue hancurin!";
    }

    public void SelectBribe()
    {
        if (Wallet.DeductMoney(bribeCost))
        {
            CloseDialogue();
        }
        else
        {
            contentText.text = "Duit gak cukup buat nyogok!";
            StartCoroutine(CloseAfterDelay(1.5f));
        }
    }

    public void SelectSurrender()
    {
        StopAllCoroutines(); 

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player == null) 
        {
            CloseDialogue();
            return;
        }

        if (player.activeDeliveryIDs.Count > 0)
        {
            player.TakePackageDamage(25f);
            CloseDialogue();
        }
        else if (Wallet.currentMoney <= 0)
        {
            caughtWithoutItemsCount++;

            if (caughtWithoutItemsCount == 1)
            {
                contentText.text = "Hah? Gak bawa paket? Gak punya duit? Pergi sana!";
                StartCoroutine(CloseAfterDelay(2f));
            }
            else
            {
                contentText.text = "Muka lu lagi! Gue pukul ya! Rasain nih pusing!";
                player.ApplyStunEffect(4f); 
                StartCoroutine(CloseAfterDelay(2.5f));
            }

            if (bribeButton != null) bribeButton.interactable = false;
        }
        else
        {
            CloseDialogue();
        }
    }

    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); 
        CloseDialogue();
    }

    public void CloseDialogue()
    {
        if(dialoguePanel != null) dialoguePanel.SetActive(false);
        Time.timeScale = 1f; 
        isDialogueActive = false;
        
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach(EnemyAI e in enemies) 
        {
            e.ResetToStartPosition();
        }
    }
}