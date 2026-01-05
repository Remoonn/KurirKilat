using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour
{
    [Header("Pengaturan Fade")]
    public Image fadePanel;
    public float kecepatanFade = 1.5f;

    [Header("Panel Opsi")]
    public GameObject panelOpsi; // Masukkan PanelOpsi di sini

    // --- 1. START (Fade In) ---
    void Start()
    {
        // Pastikan Panel Opsi tertutup saat game mulai
        if (panelOpsi != null) panelOpsi.SetActive(false);

        if (fadePanel != null)
        {
            fadePanel.raycastTarget = false;
            StartCoroutine(ProsesFadeIn());
        }
    }

    // --- 2. FUNGSI UTAMA ---
    public void TekanPlay()
    {
        fadePanel.raycastTarget = true;
        StartCoroutine(FadeOutKeSinopsis());
    }

    public void TekanKeluar()
    {
        Debug.Log("Keluar Game!");
        Application.Quit();
    }

    // --- 3. LOGIKA OPSI (BARU) ---
    
    // Dipasang di Tombol "OPSI" pada Menu Utama
    public void BukaOpsi()
    {
        if (panelOpsi != null) panelOpsi.SetActive(true);
    }

    // Dipasang di Tombol "KEMBALI/X" di dalam Panel Opsi
    public void TutupOpsi()
    {
        if (panelOpsi != null) panelOpsi.SetActive(false);
    }

    // Dipasang di Toggle Fullscreen (On Value Changed)
    public void AturFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen set to: " + isFullscreen);
    }

    // Dipasang di Slider Audio (Nanti disambung ke Audio Manager)
    public void AturVolumeMusik(float volume)
    {
        Debug.Log("Volume Musik: " + volume);
        // Nanti kita isi kode AudioMixer di sini
    }

    // --- LOGIKA FADE (TETAP SAMA) ---
    IEnumerator ProsesFadeIn()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * kecepatanFade;
            if(fadePanel != null) fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator FadeOutKeSinopsis()
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * kecepatanFade;
            if(fadePanel != null) fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        SceneManager.LoadScene("StorySynopsis");
    }
}