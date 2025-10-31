using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseUI;
    public GameObject pauseBtn;
    public GameObject resumeBtn;
    public GameObject RestartBtn;
    public GameObject ExitBtn;
    public GameObject blackOverlay;
    public GameObject HUD;

    [Header("Game UI")]
    public GameObject Joystick;
    public GameObject Jumpbtn;
    public GameObject GameOverUI;
    public GameObject PanicMeterUI;
    public GameObject CoverBtn;
    public GameObject uncoverBtm;
    public GameObject heartsys;

    [Header("Puzzle References")]
    public BreakerPuzzle breakerPuzzle;

    [Header("Fire Extinguisher References")]
    public FireExtinguisher fireExtinguisher;

    [Header("Text")]
    public TMP_Text panicText;
    public TMP_Text injurtyText;

    [Header("Scripts")]
    public PanicMeterScript panicMeterScript;
    public HeartSys hearts;
    public Movements movementscript;

    [Header("HUD Handling")]
    public CanvasGroup hudCanvasGroup;

    [Header("Narrative/UI Managers")]
    public SubtitleManager2 subtitleManager2;

    public static GameManager Instance;
    public bool isPaused = false;

    // Track visibility states
    private bool wasBreakerPuzzleVisible = false;
    private bool wasFireExtinguisherPickupVisible = false;
    private bool wasFireExtinguisherSprayVisible = false;

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
        if (isPaused) return;

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

        if (Joystick != null) Joystick.SetActive(false);
        if (Jumpbtn != null) Jumpbtn.SetActive(false);
        if (CoverBtn != null) CoverBtn.SetActive(false);
        if (uncoverBtm != null) uncoverBtm.SetActive(false);

        // 🔸 Hide pickup button properly (handles ALL pickups including oxygen)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.OnPause();

        // 🔸 Hide breaker puzzle
        if (breakerPuzzle != null && breakerPuzzle.breakerPanel != null)
        {
            wasBreakerPuzzleVisible = breakerPuzzle.breakerPanel.activeSelf;
            if (wasBreakerPuzzleVisible)
                breakerPuzzle.breakerPanel.SetActive(false);
        }

        // 🔸 Hide fire extinguisher UI
        if (fireExtinguisher != null)
        {
            if (fireExtinguisher.pickupButton != null)
            {
                wasFireExtinguisherPickupVisible = fireExtinguisher.pickupButton.activeSelf;
                if (wasFireExtinguisherPickupVisible)
                    fireExtinguisher.pickupButton.SetActive(false);
            }

            if (fireExtinguisher.sprayButton != null)
            {
                wasFireExtinguisherSprayVisible = fireExtinguisher.sprayButton.activeSelf;
                if (wasFireExtinguisherSprayVisible)
                    fireExtinguisher.sprayButton.SetActive(false);
            }
        }

        // 🔸 Hide subtitles/objectives (SubtitleManager handles resume)
        if (subtitleManager2 != null)
            subtitleManager2.OnPause();
    }

    public void Resume()
    {
        if (!isPaused) return;

        if (pauseUI != null) pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        AudioListener.pause = false;

        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);
        if (HUD != null) HUD.SetActive(true);

        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 1f;
            hudCanvasGroup.interactable = true;
            hudCanvasGroup.blocksRaycasts = true;
        }

        if (Joystick != null) Joystick.SetActive(true);
        if (Jumpbtn != null) Jumpbtn.SetActive(true);
        if (CoverBtn != null) CoverBtn.SetActive(true);
        if (uncoverBtm != null) uncoverBtm.SetActive(true);

        // 🔹 Restore pickup button (handles ALL pickups including oxygen)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.OnResume();

        // 🔹 Restore breaker puzzle if it was visible
        if (wasBreakerPuzzleVisible && breakerPuzzle != null && breakerPuzzle.breakerPanel != null)
            breakerPuzzle.breakerPanel.SetActive(true);

        // 🔹 Restore fire extinguisher UI if visible
        if (wasFireExtinguisherPickupVisible && fireExtinguisher != null && fireExtinguisher.pickupButton != null)
            fireExtinguisher.pickupButton.SetActive(true);

        if (wasFireExtinguisherSprayVisible && fireExtinguisher != null && fireExtinguisher.sprayButton != null)
            fireExtinguisher.sprayButton.SetActive(true);

        // 🔹 Resume subtitles/objectives safely
        if (subtitleManager2 != null)
            subtitleManager2.OnResume();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (DataManager.Instance.playerData != null)
        {
            var data = DataManager.Instance.playerData;
            data.totalQuestionsAnswered = 0;
            data.overallTotalScore = 0;

            foreach (var trial in data.trials)
            {
                if (trial == null) continue;

                trial.quizScore = 0;
                trial.questionsAnswered = 0;
                trial.factsDiscovered = 0;
                trial.totalScore = 0;
            }

            Debug.Log("✅ Player data fully reset (trials only, no stages)!");
        }

        DataManager.Instance.quizScore = 0;
        DataManager.Instance.factsDiscovered = 0;
        DataManager.Instance.totalQuestionsAnswered = 0;
        DataManager.Instance.Npcs_saved = 0;
    }

    IEnumerator LoadHubScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

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
        if (pauseUI != null) pauseUI.SetActive(false);
        if (heartsys != null) heartsys.SetActive(false);

        if (movementscript != null) movementscript.enabled = false;

        if (fireExtinguisher != null)
        {
            if (fireExtinguisher.pickupButton != null)
                fireExtinguisher.pickupButton.SetActive(false);
            if (fireExtinguisher.sprayButton != null)
                fireExtinguisher.sprayButton.SetActive(false);
        }

        // ✅ Hide pickup UI on game over (handles ALL pickups)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.HidePickupPrompt();

        // ✅ Also hide subtitles on death
        if (subtitleManager2 != null)
            subtitleManager2.HideAll();
    }
}