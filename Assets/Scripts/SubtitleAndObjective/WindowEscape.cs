using UnityEngine;
using TMPro;
<<<<<<< HEAD
using UnityEngine.UI; // for Image
=======
using System.Collections;
using UnityEngine.UI;
>>>>>>> 47c3962 (Quiz script changes)

public class WindowEscape : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager subtitleManager;
<<<<<<< HEAD
    public GameObject heavyObject; // Lamp or chair
    public DoorFireTrigger doorFireTrigger; // Reference to previous step

    [Header("Ending Screen")]
    public GameObject fadePanel; // UI Panel (Image) that covers screen
    public TextMeshProUGUI endingText; // Ending text
=======
    public GameObject heavyObject;
    public DoorFireTrigger doorFireTrigger;

    [Header("Ending Screen")]
    public GameObject fadePanel;
    public TextMeshProUGUI endingText;
    public TextMeshProUGUI scoreText;
>>>>>>> 47c3962 (Quiz script changes)
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
<<<<<<< HEAD
        // Add CanvasGroup to fadePanel if missing
=======
>>>>>>> 47c3962 (Quiz script changes)
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
<<<<<<< HEAD
=======

        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }
>>>>>>> 47c3962 (Quiz script changes)
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEscaped && !isFading && !promptShown)
        {
<<<<<<< HEAD
=======
            if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
            {
                return;
            }

>>>>>>> 47c3962 (Quiz script changes)
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
<<<<<<< HEAD
=======
            if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
            {
                return;
            }

>>>>>>> 47c3962 (Quiz script changes)
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

<<<<<<< HEAD
    // Called by HeavyObjectPickup script
=======
>>>>>>> 47c3962 (Quiz script changes)
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

<<<<<<< HEAD
=======
        if (scoreText != null)
        {
            DataManager dataManager = FindObjectOfType<DataManager>();
            if (dataManager != null)
            {
                int totalScore = dataManager.GetTotalScore();
                scoreText.text = $"Quiz Final Score: {totalScore}/9";
            }
            else
            {
                scoreText.text = "Quiz Final Score: 0/9";
            }

            scoreText.gameObject.SetActive(true);
        }

>>>>>>> 47c3962 (Quiz script changes)
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
<<<<<<< HEAD
        // SceneManager.LoadScene("MainMenu");
        // Application.Quit();
    }
}
=======
    }
}
>>>>>>> 47c3962 (Quiz script changes)
