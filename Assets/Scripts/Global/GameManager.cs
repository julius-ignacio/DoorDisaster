using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseUI, pauseBtn, resumeBtn, RestartBtn, ExitBtn, blackOverlay, HUD, RestartConfirmationUI; // Drag your PauseUI here in Inspector

    public GameObject Joystick, Jumpbtn, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm, PauseUI, heartsys, InventoryUI;

    public TMP_Text panicText, injurtyText;

    [Header("Scripts")]
    public PanicMeterScript panicMeterScript;
        public Movements movementscript;
    public HeartSys hearts;
        [Header("Narrative/UI Managers")]
    public SubtitleManager2 subtitleManager2;


    /// /////////////////
    [Header("Puzzle References")]
    public BreakerPuzzle breakerPuzzle;
    
        [Header("Fire Extinguisher References")]
    public FireExtinguisher fireExtinguisher;

    // Track visibility states
    private bool wasBreakerPuzzleVisible = false;
    private bool wasFireExtinguisherPickupVisible = false;
    private bool wasFireExtinguisherSprayVisible = false;

    public bool isPaused = false;


    [Header("Water")]
    public OxygenHealthMeterScript oxyHealthMeter;





/// ////////////////////////////

    [Header("HUD Handling")]
    public CanvasGroup hudCanvasGroup; // assign the HUD's CanvasGroup he



//9999999999999999999999999999999999999999999999999

    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }



/// 99999999999999999999999999999999999999
    void Start()
    {
        if (resumeBtn != null) resumeBtn.SetActive(false);
        if (ExitBtn != null) ExitBtn.SetActive(false);
        if (pauseBtn != null) pauseBtn.SetActive(true);
        if (RestartBtn != null) RestartBtn.SetActive(false);
        if (blackOverlay != null) blackOverlay.SetActive(false);
        if (InventoryUI != null) InventoryUI.SetActive(false);
        if (RestartConfirmationUI != null) RestartConfirmationUI.SetActive(false);

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

    public void RestartConfirmation()
    {
        if (RestartConfirmationUI != null)
        {
            RestartConfirmationUI.SetActive(true);
        }
    }
    
      public void NoRestartConf()
    {
        if (RestartConfirmationUI != null) {
            RestartConfirmationUI.SetActive(false); 
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

        if (oxyHealthMeter != null && oxyHealthMeter.currHealth <= 0)
        {
            if (panicText != null) panicText.gameObject.SetActive(true);
            if (injurtyText != null) injurtyText.gameObject.SetActive(true);
            playerGameOver();
        }



    }

        // Call this from the Restart button OnClick
    public void RestartLevelHard()
    {
        // Stop audio and unpause time/audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAll();
            AudioManager.Instance.StopLoop();
        }
            isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;

        var dm = DataManager.Instance;

        // Delete current trial/mode save
        if (dm != null)
        {
            WorldSaveSystem.DeleteSave(dm.currentTrial, dm.currentMode);

            // Also reset in-memory quick globals (fresh run)
            dm.ResetGlobalsForNewRun();

            // Reset this trial’s stored stats (so Almanac etc. shows zero)
            var trial = dm.playerData?.Mode?[dm.currentMode]?.trials?[dm.currentTrial];
            if (trial != null)
            {
                trial.quizScore = 0;
                trial.questionsAnswered = 0;
                trial.factsDiscovered = 0;
                trial.totalScore = 0;
            }

            // Ensure no world load happens even if some stray file exists
            dm.skipNextWorldLoad = true;
        }

        // Reload current scene (will start from initial state)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("🔁 Hard restart: deleted save, reset stats, reloaded scene.");
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
        if (InventoryUI != null) InventoryUI.SetActive(false);
        if (hudCanvasGroup != null)
        {
            hudCanvasGroup.alpha = 0f;
            hudCanvasGroup.interactable = false;
            hudCanvasGroup.blocksRaycasts = false;
        }

        // Hide pickup button properly (handles ALL pickups including oxygen)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.OnPause();


        // Hide breaker puzzle
        if (breakerPuzzle != null && breakerPuzzle.breakerPanel != null)
        {
            wasBreakerPuzzleVisible = breakerPuzzle.breakerPanel.activeSelf;
            if (wasBreakerPuzzleVisible)
                breakerPuzzle.breakerPanel.SetActive(false);
        }
        

        // Hide fire extinguisher UI
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


        // Hide subtitles/objectives (SubtitleManager handles resume)
        if (subtitleManager2 != null)
            subtitleManager2.OnPause();

    }






    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseUI.SetActive(true); // add this line


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


          // Restore pickup button (handles ALL pickups including oxygen)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.OnResume();

        // Restore breaker puzzle if it was visible
        if (wasBreakerPuzzleVisible && breakerPuzzle != null && breakerPuzzle.breakerPanel != null)
            breakerPuzzle.breakerPanel.SetActive(true);

        // Restore fire extinguisher UI if visible
        if (wasFireExtinguisherPickupVisible && fireExtinguisher != null && fireExtinguisher.pickupButton != null)
            fireExtinguisher.pickupButton.SetActive(true);

        if (wasFireExtinguisherSprayVisible && fireExtinguisher != null && fireExtinguisher.sprayButton != null)
            fireExtinguisher.sprayButton.SetActive(true);


                    // Resume subtitles/objectives safely
        if (subtitleManager2 != null)
            subtitleManager2.OnResume();
    }

    IEnumerator LoadHubScene()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        AudioManager.Instance.StopAll();
        AudioManager.Instance.StopLoop();


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
        SaveGameState();
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


                if (fireExtinguisher != null)
        {
            if (fireExtinguisher.pickupButton != null)
                fireExtinguisher.pickupButton.SetActive(false);
            if (fireExtinguisher.sprayButton != null)
                fireExtinguisher.sprayButton.SetActive(false);
        }

        // Hide pickup UI on game over (handles ALL pickups)
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.HidePickupPrompt();
        // Also hide subtitles on death
        if (subtitleManager2 != null)
            subtitleManager2.HideAll();
    }







    private void OnApplicationQuit()
    {
        SaveGameState();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGameState();
    }

    public void SaveGameState()
    {
        int trialIndex = DataManager.Instance.currentTrial;
        int mode = DataManager.Instance.currentMode;

        // Save player progress (quiz/facts/etc.)
        DataManager.Instance.SaveTrialData(trialIndex, DataManager.Instance.currentMode);

        // Save world objects
        WorldSaveSystem.SaveWorld(trialIndex, mode);

        Debug.Log($"Game saved for {mode} Trial {trialIndex}");
    }
}
























