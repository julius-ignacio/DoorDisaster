using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseUI, pauseBtn, resumeBtn, ExitBtn, blackOverlay, HUD; // Drag your PauseUI here in Inspector
    private bool isPaused = false;

     public GameObject Joystick, Jumpbtn, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm, PauseUI, heartsys;
    public PanicMeterScript panicMeterScript;
    public HeartSys hearts;
    public TMP_Text panicText, injurtyText;
    public Movements movementscript;


    void Start()
    {
        resumeBtn.SetActive(false);
        ExitBtn.SetActive(false);
        pauseBtn.SetActive(true); // Ensure pause UI is hidden at start
        blackOverlay.SetActive(false); // Ensure black overlay is hidden at start
    }

    void Update()
    {
        PlayerInjured_Death();
        // Optional: Allow pressing ESC/ P to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void PlayerInjured_Death()
    {
     if (panicMeterScript.currHealth >= 100)
        {
            panicText.gameObject.SetActive(true);
            injurtyText.gameObject.SetActive(false);
            playerGameOver();
        }

        if (hearts.currentHearts <= 0)
        {
            panicText.gameObject.SetActive(false);
            injurtyText.gameObject.SetActive(true);
            playerGameOver();
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

    public void RestartLevel()
    {
        // Unpause
        Time.timeScale = 1f;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
    

    public void playerGameOver()
    {
        Joystick.SetActive(false);
        Jumpbtn.SetActive(false);
        GameOverUI.SetActive(true);
        PanicMeterUI.SetActive(false);
        CoverBtn.SetActive(false);
        uncoverBtm.SetActive(false);
        PauseUI.SetActive(false);
        heartsys.SetActive(false);

        movementscript.enabled = false;

    }
}
