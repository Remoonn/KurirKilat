using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string npcName;
    public Sprite npcFaceImage; // Simpan gambar wajah NPC di sini
    
    [Header("Dialog Text Settings")]
    [TextArea(3, 5)] 
    public string normalGreeting = "Halo! Senang melihatmu hari ini.";
    
    [TextArea(3, 5)] 
    public string missionGreeting = "Bisakah kamu membantuku mengantar paket ini ke tujuan?";

    private bool isPlayerInRange = false;

    void Update()
    {
        // Interaksi tombol E
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Interact()
    {
        UIManager ui = FindObjectOfType<UIManager>();
        PickupPoint pickup = GetComponent<PickupPoint>();
        PlayerMovement player = FindObjectOfType<PlayerMovement>();

        if (ui == null || player == null) return;

        player.StopMoving(); // Cegah player berjalan saat dialog aktif

        // Cek apakah NPC ini pemberi misi yang sedang aktif
        if (pickup != null && pickup.enabled && pickup.IsCurrentMissionActive())
        {
            // Kirim Nama, Teks, Referensi Misi, dan Gambar ke UI
            ui.OpenMissionOffer(npcName, missionGreeting, pickup, npcFaceImage);
        }
        else
        {
            // Kirim Nama, Teks, dan Gambar ke UI
            ui.OpenSimpleDialogue(npcName, normalGreeting, npcFaceImage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}