using UnityEngine;

public class GameMusicController : MonoBehaviour
{
    [Header("Masukkan File Musik Di Sini")]
    public AudioClip musikSantai; // Drag lagu santai (mp3) ke sini
    public AudioClip musikPanik;  // Drag lagu panik (mp3) ke sini

    [Header("Pengaturan")]
    public float kecepatanTransisi = 2.0f; 

    // Audio Source yang dibuat otomatis oleh script
    private AudioSource sourceSantai;
    private AudioSource sourcePanik;
    
    private bool sedangDikejar = false;

    void Start()
    {
        // Setup Otomatis: Membuat 2 pemutar musik
        sourceSantai = gameObject.AddComponent<AudioSource>();
        sourcePanik = gameObject.AddComponent<AudioSource>();

        // Setting musik Santai
        sourceSantai.clip = musikSantai;
        sourceSantai.loop = true;
        sourceSantai.volume = 1f; // Mulai nyala
        sourceSantai.Play();

        // Setting musik Panik
        sourcePanik.clip = musikPanik;
        sourcePanik.loop = true;
        sourcePanik.volume = 0f; // Mulai mati (silent)
        sourcePanik.Play();
    }

    void Update()
    {
        // LOGIKA TRANSISI VOLUME (CROSSFADE)
        if (sedangDikejar)
        {
            // Jika dikejar: Santai pelan2 mati, Panik pelan2 nyala
            sourceSantai.volume = Mathf.MoveTowards(sourceSantai.volume, 0f, Time.deltaTime * kecepatanTransisi);
            sourcePanik.volume = Mathf.MoveTowards(sourcePanik.volume, 1f, Time.deltaTime * kecepatanTransisi);
        }
        else
        {
            // Jika aman: Santai pelan2 nyala, Panik pelan2 mati
            sourceSantai.volume = Mathf.MoveTowards(sourceSantai.volume, 1f, Time.deltaTime * kecepatanTransisi);
            sourcePanik.volume = Mathf.MoveTowards(sourcePanik.volume, 0f, Time.deltaTime * kecepatanTransisi);
        }
    }

    // Fungsi ini dipanggil oleh EnemyAI
    public void SetStatusDikejar(bool status)
    {
        sedangDikejar = status;
    }
}