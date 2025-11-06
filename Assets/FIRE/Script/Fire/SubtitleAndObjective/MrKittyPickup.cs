using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MrKittyPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject cat;
    public SubtitleManager2 subtitleManager;
    public FireSafetyQuiz quizManager;

    [Header("Teleport Settings")]
    public Transform player;
    public Transform houseASpawnPoint;

    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;

    private bool hasTriggered = false;
    private bool playerInRange = false;

    void Start()
    {
        // Ensure fade overlay starts invisible
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInRange = true;

            // Don't show prompt if game is paused
            if (GameManager.Instance == null || !GameManager.Instance.isPaused)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Rescue Mr. Kitty");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GenericPickupButton.Instance.HidePickupPrompt();
        }
    }

    void Update()
    {
        // ✅ Show prompt when player is in range and game resumes
        if (playerInRange && !hasTriggered)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isPaused)
            {
                // Check if prompt isn't already showing
                if (GenericPickupButton.Instance != null &&
                    GenericPickupButton.Instance.pickupButton != null &&
                    !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Rescue Mr. Kitty");
                }
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasTriggered) return;

        hasTriggered = true;
        subtitleManager.HideObjective();
        GenericPickupButton.Instance.HidePickupPrompt();

        // Start the sequence - cat stays visible
        subtitleManager.ShowCustomMessage(
            "Come on Mr. Kitty, let's get you to safety!",
            2f,
            () => StartCoroutine(FadeTeleportSequence())
        );
    }

    private IEnumerator FadeTeleportSequence()
    {
        // 1️⃣ Fade out
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // 2️⃣ Teleport player back to House A
        if (player != null && houseASpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (cc != null) cc.enabled = false;

            player.position = houseASpawnPoint.position;
            player.rotation = houseASpawnPoint.rotation;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogError("Teleport failed: Player or House A Spawn Point not assigned!");
            yield break;
        }

        // 🔥 RESUME FIRE SPREADING AFTER TELEPORT
        if (FireSpreadManager.Instance != null)
        {
            FireSpreadManager.Instance.ResumeFireSpread();
            Debug.Log("MrKittyPickup: Resumed fire spreading in House A");
        }

        yield return new WaitForSeconds(0.2f);

        // 3️⃣ Fade in
        if (fadeOverlay != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
            fadeOverlay.gameObject.SetActive(false);
        }

        // 4️⃣ Continue subtitle sequence
        subtitleManager.ShowCustomMessage(
            "Wait... I'm back? How did I get here?",
            3f,
            () =>
            {
                subtitleManager.ShowCustomMessage(
                    "Mr. Kitty is safe... but something feels off.",
                    3f,
                    () =>
                    {
                        // 5️⃣ Show quiz
                        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("pet_hiding");
                        if (quiz != null && quizManager != null)
                        {
                            quizManager.ShowQuiz(
                                quiz.question,
                                quiz.answers,
                                quiz.correctAnswerIndex,
                                () => OnQuizComplete()
                            );
                        }
                        else
                        {
                            Debug.LogError("Quiz 'pet_hiding' not found!");
                            OnQuizComplete();
                        }
                    }
                );
            }
        );
    }

    private void OnQuizComplete()
    {
        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "I need to escape now!",
                2f,
                () =>
                {
                    // Start the packing objective
                    ObjectiveManager objManager = FindObjectOfType<ObjectiveManager>();
                    if (objManager != null)
                    {
                        objManager.StartPackingObjective();
                    }
                    else
                    {
                        Debug.LogError("ObjectiveManager not found in scene!");
                        subtitleManager.ShowObjective("Find the nearest exit to escape the fire");
                    }
                }
            );
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color color = fadeOverlay.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeOverlay.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    public bool HasReachedCat()
    {
        return hasTriggered;
    }
}