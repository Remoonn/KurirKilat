using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // Menghentikan gerakan musuh dan player
        isPaused = true;
    }
}