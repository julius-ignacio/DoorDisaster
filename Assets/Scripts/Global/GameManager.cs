using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
      [Header("UI References")]
    public GameObject pauseUI, pauseBtn, resumeBtn, RestartBtn, ExitBtn, blackOverlay, HUD; // Drag your PauseUI here in Inspector

     public GameObject Joystick, Jumpbtn, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm, PauseUI, heartsys;
    public TMP_Text panicText, injurtyText;

    [Header("Scripts")]
    public PanicMeterScript panicMeterScript;
    public HeartSys hearts;
    public Movements movementscript;

    public static GameManager Instance;


    public bool isPaused = false;


        [Header("HUD Handling")]
    public CanvasGroup hudCanvasGroup; // assign the HUD's CanvasGroup he



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
        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);

          // Make sure HUD starts visible
        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 1f;
            hudCanvasGroup.interactable = true;
            hudCanvasGroup.blocksRaycasts = true;
        }
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



                if (panicMeterScript != null && panicMeterScript.currHealth >= 100)
        {
            if (panicText != null) panicText.gameObject.SetActive(true);
            if (injurtyText != null) injurtyText.gameObject.SetActive(false);
            playerGameOver();
        }

        if (hearts != null && hearts.currentHearts <= 0)
        {
            if (panicText != null) panicText.gameObject.SetActive(false);
            if (injurtyText != null) injurtyText.gameObject.SetActive(true);
            playerGameOver();
        }
        

}





    public void Pause()
    {
        if (pauseUI != null) pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        AudioListener.pause = true; // 🔇 Pause ALL audio in the scene


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
    

    

public void RestartLevel()
{
    // Unpause
    Time.timeScale = 1f;

    // Reload current scene
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    // Reset DataManager
    if (DataManager.Instance.playerData != null)
    {
        var data = DataManager.Instance.playerData;
        data.totalQuestionsAnswered = 0;
        data.overallTotalScore = 0;

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


    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;

        AudioListener.pause = false; // 🔊 Resume ALL audio


        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);
        if (HUD != null) HUD.SetActive(true);

             // Show HUD again
        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 1f;
            hudCanvasGroup.interactable = true;
            hudCanvasGroup.blocksRaycasts = true;
        }
    }

IEnumerator LoadHubScene()
{
    Time.timeScale = 1f;
    AudioListener.pause = false;

    CanvasGroup overlayGroup = blackOverlay.GetComponent<CanvasGroup>();
    blackOverlay.SetActive(true);

    // Gradually fade to black over 0.5 seconds
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


public void ExitGame()
{
    StartCoroutine(LoadHubScene());
}

    

    public void playerGameOver()
    {
        if (Joystick != null) Joystick.SetActive(false);
        if (Jumpbtn != null) Jumpbtn.SetActive(false);
        if (GameOverUI != null) GameOverUI.SetActive(true);
        if (PanicMeterUI != null) PanicMeterUI.SetActive(false);
        if (CoverBtn != null) CoverBtn.SetActive(false);
        if (uncoverBtm != null) uncoverBtm.SetActive(false);
        if (PauseUI != null) PauseUI.SetActive(false);
        if (heartsys != null) heartsys.SetActive(false);

        if (movementscript != null) movementscript.enabled = false;

    }
}
