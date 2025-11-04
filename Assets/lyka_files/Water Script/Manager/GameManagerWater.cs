using SojaExiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManagerWater : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseUI, pauseBtn, resumeBtn, RestartBtn, ExitBtn, blackOverlay, HUD;
    public GameObject Joystick, Jumpbtn, GameOverUI, PauseUI, heartsys;
    public TMP_Text drowningText; // Appears when player drowns

    [Header("Scripts")]
    public HeartSysWater hearts;
    public PlayerController_Water playerController;

    [Header("Audio")]
    public AudioSource eatingSound; // sound when player eats a snack

    [Header("HUD Handling")]
    public CanvasGroup hudCanvasGroup; // assign the HUD's CanvasGroup here in Inspector

    public static GameManagerWater Instance;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize UI state
        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);

        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 1f;
            hudCanvasGroup.interactable = true;
            hudCanvasGroup.blocksRaycasts = true;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        CheckDrowningDeath();

        // Toggle pause on ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // 🫧 Check if player has drowned
    void CheckDrowningDeath()
    {
        if (hearts != null && hearts.currentHearts <= 0)
        {
            if (drowningText != null) drowningText.gameObject.SetActive(true);
            PlayerGameOver();
        }
    }

    // 🕹️ Pause the game
    public void Pause()
    {
        if (pauseUI != null) pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        AudioListener.pause = true;

        if (resumeBtn != null) resumeBtn.SetActive(true);
        if (ExitBtn != null) ExitBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(true);
        if (pauseBtn != null) pauseBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(true);

        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 0f;
            hudCanvasGroup.interactable = false;
            hudCanvasGroup.blocksRaycasts = false;
        }
    }

    // ▶️ Resume the game
    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        AudioListener.pause = false;

        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (blackOverlay != null) blackOverlay.SetActive(false);

        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 1f;
            hudCanvasGroup.interactable = true;
            hudCanvasGroup.blocksRaycasts = true;
        }
    }

    // 🔁 Restart the current level and reset data
    public void RestartLevel()
    {
    
    AudioManager.Instance.StopAll();
AudioManager.Instance.StopLoop();

        // Unpause
        Time.timeScale = 1f;
    AudioListener.pause = false;


    // Reload current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    // Reset DataManager
    if (DataManager.Instance.playerData != null)
    {
        var data = DataManager.Instance.playerData;

        foreach (var trial in data.trials)
        {
            if (trial == null) continue;

            // Reset trial-level stats
            trial.quizScore = 0;
            trial.questionsAnswered = 0;
            trial.factsDiscovered = 0;
            trial.totalScore = 0;

        }

        Debug.Log("✅ Player data fully reset (trials only, no stages)!");
    }

    // Reset quick-access globals
    DataManager.Instance.quizScore = 0;
    DataManager.Instance.factsDiscovered = 0;
    DataManager.Instance.totalQuestionsAnswered = 0;
    DataManager.Instance.Npcs_saved = 0;
}

    // 🌊 Exit back to Temple hub (fade out)
    public void ExitGame()
    {
        StartCoroutine(LoadHubScene());
    }

    private IEnumerator LoadHubScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        AudioManager.Instance.StopAll();
        AudioManager.Instance.StopLoop();

        CanvasGroup overlayGroup = blackOverlay.GetComponent<CanvasGroup>();
        blackOverlay.SetActive(true);

        float fadeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            overlayGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        SceneManager.LoadScene("Temple");
    }

    // 💀 When the player drowns
    public void PlayerGameOver()
    {
        if (Joystick != null) Joystick.SetActive(false);
        if (Jumpbtn != null) Jumpbtn.SetActive(false);
        if (GameOverUI != null) GameOverUI.SetActive(true);
        if (PauseUI != null) PauseUI.SetActive(false);
        if (heartsys != null) heartsys.SetActive(false);

        if (playerController != null) playerController.enabled = false;
    }

    // 🍪 Called when player eats a snack
    public void EatSnack()
    {
        if (hearts != null && hearts.currentHearts < hearts.maxHearts)
        {
            hearts.currentHearts += 1;
            if (eatingSound != null) eatingSound.Play();
        }
    }
}
