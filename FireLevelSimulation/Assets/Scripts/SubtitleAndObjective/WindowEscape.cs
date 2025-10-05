using UnityEngine;
using TMPro;
using UnityEngine.UI; // for Image

public class WindowEscape : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager subtitleManager;
    public GameObject heavyObject; // Lamp or chair
    public DoorFireTrigger doorFireTrigger; // Reference to previous step

    [Header("Ending Screen")]
    public GameObject fadePanel; // UI Panel (Image) that covers screen
    public TextMeshProUGUI endingText; // Ending text
    public float fadeDuration = 2f;

    private bool hasHeavyObject = false;
    private bool hasEscaped = false;
    private bool isFading = false;
    private bool promptShown = false;
    private bool quizDone = false;

    private CanvasGroup fadeCanvasGroup;
    private CanvasGroup textCanvasGroup;

    void Start()
    {
        // Add CanvasGroup to fadePanel if missing
        if (fadePanel != null)
        {
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
            if (fadeCanvasGroup == null)
                fadeCanvasGroup = fadePanel.AddComponent<CanvasGroup>();

            fadePanel.SetActive(false);
            fadeCanvasGroup.alpha = 0;
        }

        if (endingText != null)
        {
            textCanvasGroup = endingText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
                textCanvasGroup = endingText.gameObject.AddComponent<CanvasGroup>();

            endingText.gameObject.SetActive(false);
            textCanvasGroup.alpha = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEscaped && !isFading && !promptShown)
        {
            promptShown = true;
            subtitleManager.ShowCustomMessage(
                "Let's try to open this window...",
                2f,
                () => subtitleManager.ShowObjective("Press E to try opening the window")
            );
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasEscaped && !isFading)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (hasHeavyObject)
                {
                    StartEscape();
                }
                else if (!quizDone)
                {
                    subtitleManager.ShowCustomMessage(
                        "Oh no, it's stuck!",
                        2f,
                        () =>
                        {
                            quizDone = true;
                            subtitleManager.ShowObjective("Find a heavy object - try the lamp near the bed");
                        }
                    );
                }
                else
                {
                    subtitleManager.ShowCustomMessage(
                        "I need something heavy to break this open!",
                        2f
                    );
                }
            }
        }
    }

    // Called by HeavyObjectPickup script
    public void PickupHeavyObject()
    {
        hasHeavyObject = true;

        if (heavyObject != null)
            heavyObject.SetActive(false);

        subtitleManager.HideObjective();
        subtitleManager.ShowCustomMessage(
            "Got it! This should break the window!",
            2f,
            () => subtitleManager.ShowObjective("Use the heavy object to break the bedroom window")
        );
    }

    void StartEscape()
    {
        hasEscaped = true;
        subtitleManager.HideObjective();

        ShowFinalScreen();
    }

    void ShowFinalScreen()
    {
        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
            StartCoroutine(FadeCanvas(fadeCanvasGroup, 0f, 1f, fadeDuration));
        }

        if (endingText != null)
        {
            endingText.text = "YOU ESCAPED!\n\nRemember: Always have an escape plan!";
            endingText.gameObject.SetActive(true);
            StartCoroutine(FadeCanvas(textCanvasGroup, 0f, 1f, fadeDuration));
        }

        StartCoroutine(FadeOutAndEnd());
    }

    System.Collections.IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float duration)
    {
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

    System.Collections.IEnumerator FadeOutAndEnd()
    {
        isFading = true;

        yield return new WaitForSeconds(fadeDuration + 1f);

        Debug.Log("Game Complete!");
        // SceneManager.LoadScene("MainMenu");
        // Application.Quit();
    }
}
