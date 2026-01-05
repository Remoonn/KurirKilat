using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro; // WAJIB ADA: Karena kita pakai TextMeshPro
using System.Collections;

public class Synopsis : MonoBehaviour
{
    [Header("Komponen UI")]
    public Image fadePanel;       // Panel Hitam
    public TMP_Text textCerita;   // Object TextMeshPro yang berisi cerita
    public GameObject tombolLanjut; // Tombol "Blanjut" / "Mulai Kerja"

    [Header("Pengaturan")]
    public float kecepatanKetik = 0.05f; // Semakin kecil semakin cepat
    public float kecepatanFade = 1.0f;
    public string namaSceneGame = "Maps"; // Pastikan nama scene benar

    private string isiCeritaLengkap; // Wadah untuk menyimpan teks asli

    void Start()
    {
        // 1. Persiapan Awal
        if (fadePanel != null) 
        {
            fadePanel.raycastTarget = false; // Biar gak ngalangin mouse
            fadePanel.color = Color.black; // Mulai dari hitam pekat
        }

        // Simpan teks yang sudah kamu tulis di Unity, lalu kosongkan layarnya
        if (textCerita != null)
        {
            isiCeritaLengkap = textCerita.text;
            textCerita.text = ""; 
        }

        // Sembunyikan tombol lanjut dulu biar rapi
        if (tombolLanjut != null) tombolLanjut.SetActive(false);

        // 2. Mulai Rangkaian Animasi (Fade In -> Ngetik)
        StartCoroutine(MulaiSequence());
    }

    // --- RANGKAIAN ANIMASI OTOMATIS ---
    IEnumerator MulaiSequence()
    {
        // Langkah A: Fade In (Layar jadi terang)
        yield return StartCoroutine(ProsesFade(1f, 0f));

        // Langkah B: Mulai Mengetik Teks
        yield return StartCoroutine(EfekMengetik());

        // Langkah C: Munculkan Tombol Lanjut setelah selesai ngetik
        if (tombolLanjut != null) tombolLanjut.SetActive(true);
    }

    // --- EFEK MENGETIK ---
    IEnumerator EfekMengetik()
    {
        foreach (char huruf in isiCeritaLengkap.ToCharArray())
        {
            textCerita.text += huruf; // Tambahkan satu huruf
            yield return new WaitForSeconds(kecepatanKetik); // Tunggu sebentar
        }
    }

    // --- FUNGSI TOMBOL LANJUT ---
    public void StartGame()
    {
        StartCoroutine(ProsesKeluar());
    }

    IEnumerator ProsesKeluar()
    {
        // Blokir klik mouse biar gak spam tombol
        if (fadePanel != null) fadePanel.raycastTarget = true;

        // Fade Out (Layar jadi Gelap)
        yield return StartCoroutine(ProsesFade(0f, 1f));

        // Pindah Scene
        SceneManager.LoadScene("Maps");
    }

    // --- LOGIKA FADE (Bisa dipakai untuk In dan Out) ---
    IEnumerator ProsesFade(float startAlpha, float endAlpha)
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * kecepatanFade;
            if (fadePanel != null)
            {
                // Lerp membuat perubahan angka jadi halus (tidak kaku)
                float alphaSaatIni = Mathf.Lerp(startAlpha, endAlpha, time);
                fadePanel.color = new Color(0, 0, 0, alphaSaatIni);
            }
            yield return null;
        }
    }
}