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
    public GameObject darkOverlay; // Simple dark image overlay

    [Header("References")]
    public Movements playerMovement;
    public PlayerOxygen playerOxygen;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f; // Duration of fade

    private CanvasGroup panelCanvasGroup;
    private CanvasGroup overlayCanvasGroup;
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup reasonCanvasGroup;

    private static GameOverManager instance;
    private bool isGameOver = false;

    void Awake()
    {
        // Singleton pattern
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

    void Update()
    {
        if (isGameOver) return;

        // Check for death conditions separately
        if (playerOxygen != null && !playerOxygen.IsAlive())
        {
            TriggerGameOver("OUT OF OXYGEN", "You ran out of breathable air. Remember: Use a wet towel to filter smoke!");
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

    System.Collections.IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        float elapsed = 0f;
        cg.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        cg.alpha = to;
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        Time.timeScale = 1f; // Reset time scale in case it was paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
