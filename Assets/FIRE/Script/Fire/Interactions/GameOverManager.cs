using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverTitle;
    public TextMeshProUGUI deathReasonText;
    public Button restartButton;
    public GameObject darkOverlay;

    [Header("References")]
    public Movements2 playerMovement;
    public PlayerOxygen playerOxygen;

    [Header("UI to Hide on Death")]
    public GameObject pauseButton;
    public GameObject inventoryButton;
    public GameObject tutorialButton;
    public GameObject healthBar;
    public GameObject oxygenBar;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private CanvasGroup panelCanvasGroup;
    private CanvasGroup overlayCanvasGroup;
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup reasonCanvasGroup;

    private static GameOverManager instance;
    private bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("GameOverManager instance created");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Setup CanvasGroups
        SetupCanvasGroup(gameOverPanel, out panelCanvasGroup);
        SetupCanvasGroup(darkOverlay, out overlayCanvasGroup);
        SetupCanvasGroup(gameOverTitle.gameObject, out titleCanvasGroup);
        SetupCanvasGroup(deathReasonText.gameObject, out reasonCanvasGroup);

        // Hide game over elements at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("GameOverPanel hidden at start");
        }
        else
        {
            Debug.LogError("GameOverPanel is NULL!");
        }

        if (darkOverlay != null)
        {
            darkOverlay.SetActive(false);
            Debug.Log("DarkOverlay hidden at start");
        }
        else
        {
            Debug.LogError("DarkOverlay is NULL!");
        }

        if (gameOverTitle != null)
        {
            gameOverTitle.gameObject.SetActive(false);
            Debug.Log("GameOverTitle hidden at start");
        }
        else
        {
            Debug.LogError("GameOverTitle is NULL!");
        }

        if (deathReasonText != null)
        {
            deathReasonText.gameObject.SetActive(false);
            Debug.Log("DeathReasonText hidden at start");
        }
        else
        {
            Debug.LogError("DeathReasonText is NULL!");
        }

        // Setup button listener
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("Restart button listener added");
        }
        else
        {
            Debug.LogError("RestartButton is NULL!");
        }
    }

    void SetupCanvasGroup(GameObject obj, out CanvasGroup cg)
    {
        if (obj == null)
        {
            cg = null;
            Debug.LogWarning("SetupCanvasGroup called with NULL object!");
            return;
        }

        cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = obj.AddComponent<CanvasGroup>();
            Debug.Log("CanvasGroup added to " + obj.name);
        }
        cg.alpha = 0f;
    }

    public void TriggerGameOver(string title, string reason)
    {
        Debug.Log("TriggerGameOver called! Title: " + title);

        if (isGameOver)
        {
            Debug.Log("Already game over, returning");
            return;
        }

        isGameOver = true;
        Debug.Log("Setting isGameOver to true");

        // Hide all UI elements
        HideGameUI();

        // Disable player controls
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            CharacterController controller = playerMovement.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;
            Debug.Log("Player movement disabled");
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor shown");

        // Start fade-in sequence
        StartCoroutine(GameOverSequence(title, reason));
    }

    void HideGameUI()
    {
        // Hide pickup button if visible
        if (GenericPickupButton.Instance != null)
            GenericPickupButton.Instance.HidePickupPrompt();

        // Close inventory panel if open
        if (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.inventoryPanel != null)
            InventoryManager_fire.Instance.inventoryPanel.SetActive(false);

        // Hide pause button
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
            Debug.Log("Pause button hidden");
        }

        // Hide inventory/backpack button
        if (inventoryButton != null)
        {
            inventoryButton.SetActive(false);
            Debug.Log("Inventory button hidden");
        }

        // ✅ Hide tutorial button
        if (tutorialButton != null)
        {
            tutorialButton.SetActive(false);
            Debug.Log("Tutorial button hidden");
        }

        // Hide health bar
        if (healthBar != null)
        {
            healthBar.SetActive(false);
            Debug.Log("Health bar hidden");
        }

        // Hide oxygen bar
        if (oxygenBar != null)
        {
            oxygenBar.SetActive(false);
            Debug.Log("Oxygen bar hidden");
        }

        // ✅ Hide subtitles/objectives
        SubtitleManager2 subtitleManager = FindObjectOfType<SubtitleManager2>();
        if (subtitleManager != null)
        {
            subtitleManager.HideAll();
            Debug.Log("Subtitles/objectives hidden");
        }
    }

    System.Collections.IEnumerator GameOverSequence(string title, string reason)
    {
        Debug.Log("GameOverSequence started");

        // Activate objects
        if (darkOverlay != null) darkOverlay.SetActive(true);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverTitle != null) gameOverTitle.gameObject.SetActive(true);
        if (deathReasonText != null) deathReasonText.gameObject.SetActive(true);

        // Set text
        if (gameOverTitle != null) gameOverTitle.text = title;
        if (deathReasonText != null) deathReasonText.text = reason;

        // --- Step 1: Fade in dark overlay ---
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            if (overlayCanvasGroup != null) overlayCanvasGroup.alpha = alpha;

            yield return null;
        }
        if (overlayCanvasGroup != null) overlayCanvasGroup.alpha = 1f;
        Debug.Log("Dark overlay faded in");

        // --- Step 2: Fade in panel + title + reason together ---
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);

            if (panelCanvasGroup != null) panelCanvasGroup.alpha = alpha;
            if (titleCanvasGroup != null) titleCanvasGroup.alpha = alpha;
            if (reasonCanvasGroup != null) reasonCanvasGroup.alpha = alpha;

            yield return null;
        }

        // Ensure final alpha = 1
        if (panelCanvasGroup != null) panelCanvasGroup.alpha = 1f;
        if (titleCanvasGroup != null) titleCanvasGroup.alpha = 1f;
        if (reasonCanvasGroup != null) reasonCanvasGroup.alpha = 1f;

        Debug.Log("GameOver panel, title, and reason faded in");
    }

    public void RestartGame()
    {
        Debug.Log("🔄 Restarting game - FULL HARD RESET");

        // Stop audio and unpause time/audio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAll();
            AudioManager.Instance.StopLoop();
        }
        Time.timeScale = 1f;
        AudioListener.pause = false;

        var dm = DataManager.Instance;

        // ✅ Get trial/mode info
        int trialIndex = (dm != null) ? dm.currentTrial : 0;
        int mode = (dm != null) ? dm.currentMode : 0;

        // ✅ CLEAR SAVE AND RESET ALL FLAGS (same as GameManager.RestartLevelHard)
        WorldSaveSystem.ClearSaveForNewTrial(trialIndex, mode);
        Debug.Log($"🔄 Called ClearSaveForNewTrial({trialIndex}, {mode})");

        if (dm != null)
        {
            // Reset in-memory quick globals (fresh run)
            dm.ResetGlobalsForNewRun();

            // Reset this trial's stored stats (so Almanac shows zero)
            var trial = dm.playerData?.Mode?[dm.currentMode]?.trials?[dm.currentTrial];
            if (trial != null)
            {
                trial.quizScore = 0;
                trial.questionsAnswered = 0;
                trial.factsDiscovered = 0;
                trial.totalScore = 0;
            }

            // Ensure no world load happens
            dm.skipNextWorldLoad = true;
        }

        // ✅ Diagnostic check
        Debug.Log($"After clear: HasSaveData = {WorldSaveSystem.HasSaveData(trialIndex, mode)}");
        Debug.Log($"After clear: SavedObjectiveStage = {ObjectiveManager.SavedObjectiveStage}");
        Debug.Log($"After clear: ItemCount = {ItemPickup.GetPickedUpCount()}");
        Debug.Log($"After clear: DoorFireShown = {DoorFireTrigger.FireMessageShown}");
        Debug.Log($"After clear: CatRescued = {MrKittyPickup.CatRescued}");
        Debug.Log($"After clear: WindowTried = {WindowEscape.WindowTried}");

        // ✅ Reload scene from the beginning (wake-up story)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("🔁 Full hard restart complete - returning to wake-up story");
    }

    // Static method so other scripts can call it easily
    public static void TriggerDeath(string title, string reason)
    {
        Debug.Log("Static TriggerDeath called");

        if (instance != null)
        {
            Debug.Log("Instance found, calling TriggerGameOver");
            instance.TriggerGameOver(title, reason);
        }
        else
        {
            Debug.LogError("GameOverManager instance is NULL!");
        }
    }
}