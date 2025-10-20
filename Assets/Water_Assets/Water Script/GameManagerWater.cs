using SojaExiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerWater : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseUI, pauseBtn, resumeBtn, ExitBtn, blackOverlay, HUD;
    public GameObject Joystick, Jumpbtn, GameOverUI, PauseUI, heartsys;
    public TMP_Text injurtyText;

    [Header("Scripts")]
    public HeartSysWater hearts;
    public PlayerMovement movementscript;

    private bool isPaused = false;

    void Start()
    {
        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (blackOverlay != null) blackOverlay.SetActive(false);
    }

    void Update()
    {
        PlayerInjured_Death();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    void PlayerInjured_Death()
    {
        if (hearts != null && hearts.currentHearts <= 0)
        {
            if (injurtyText != null) injurtyText.gameObject.SetActive(true);
            playerGameOver();
        }
    }

    public void Pause()
    {
        if (pauseUI != null) pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (resumeBtn != null) resumeBtn.SetActive(true);
        if (ExitBtn != null) ExitBtn.SetActive(true);
        if (pauseBtn != null) pauseBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(true);
        if (HUD != null) HUD.SetActive(false);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (blackOverlay != null) blackOverlay.SetActive(false);
        if (HUD != null) HUD.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void playerGameOver()
    {
        if (Joystick != null) Joystick.SetActive(false);
        if (Jumpbtn != null) Jumpbtn.SetActive(false);
        if (GameOverUI != null) GameOverUI.SetActive(true);
        if (PauseUI != null) PauseUI.SetActive(false);
        if (heartsys != null) heartsys.SetActive(false);

        if (movementscript != null) movementscript.enabled = false;
    }
}
