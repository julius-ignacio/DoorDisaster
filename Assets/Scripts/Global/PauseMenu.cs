using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SceneManagement; // for exiting to menu if needed

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI, pauseBtn, resumeBtn, ExitBtn, blackOverlay, HUD; // Drag your PauseUI here in Inspector
    private bool isPaused = false;


    void Start()
    {
        resumeBtn.SetActive(false);
        ExitBtn.SetActive(false);
        pauseBtn.SetActive(true); // Ensure pause UI is hidden at start
        blackOverlay.SetActive(false); // Ensure black overlay is hidden at start
    }

    void Update()
    {
        // Optional: Allow pressing ESC/ P to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);   // Show pause menu
        Time.timeScale = 0f;       // Stop the game
        isPaused = true;

        resumeBtn.SetActive(true);
        ExitBtn.SetActive(true);
        pauseBtn.SetActive(false); // 
        blackOverlay.SetActive(true);
         HUD.SetActive(false);
    }

    public void Resume()
    {
        Time.timeScale = 1f;       // Resume game
        isPaused = false;


        resumeBtn.SetActive(false);
        ExitBtn.SetActive(false);
        pauseBtn.SetActive(true);
        blackOverlay.SetActive(false);
                HUD.SetActive(true);
    }

    public void ExitGame()
    {
        // If you want to quit the application
        Application.Quit();

        // Or load back to main menu
        // SceneManager.LoadScene("MainMenu");
    }
}
