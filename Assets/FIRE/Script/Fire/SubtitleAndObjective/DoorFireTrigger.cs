using UnityEngine;

public class DoorFireTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public FireSafetyQuiz quizManager;
    public ObjectiveManager objectiveManager;

    private bool fireMessageShown = false;

    // ✅ Static flag for persistence across saves/restarts
    public static bool FireMessageShown { get; private set; } = false;

    void Start()
    {
        // ✅ Restore state from static flag
        fireMessageShown = FireMessageShown;

        if (fireMessageShown)
        {
            Debug.Log("✅ DoorFireTrigger restored: Fire message already shown");

            // ✅ Restore the correct objective if fire was already triggered
            if (subtitleManager != null)
            {
                // Small delay to ensure ObjectiveManager has initialized
                Invoke(nameof(RestoreFireObjective), 0.1f);
            }
        }
    }

    private void RestoreFireObjective()
    {
        // ✅ Show the alternative escape objective silently
        if (subtitleManager != null)
        {
            subtitleManager.ShowObjective("Find an alternative escape route - try the window!");
            Debug.Log("✅ Restored fire-blocked objective");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!fireMessageShown && objectiveManager != null && objectiveManager.GetObjectiveStage() >= 2)
            {
                TriggerFireSequence();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (!fireMessageShown)
            {
                if (objectiveManager != null && objectiveManager.GetObjectiveStage() >= 2)
                {
                    TriggerFireSequence();
                }
                else
                {
                    subtitleManager.ShowCustomMessage("I need to collect my essentials first!", 2f);
                }
            }
        }
    }

    private void TriggerFireSequence()
    {
        fireMessageShown = true;
        FireMessageShown = true; // ✅ Update static flag

        subtitleManager.HideObjective();

        subtitleManager.ShowCustomMessage(
            "The door is blocked by fire!",
            2.5f,
            () =>
            {
                subtitleManager.ShowObjective("Find an alternative escape route - try the window!");

                QuizQuestion2 quiz = QuizDatabase2.GetQuiz("fire_blocked_door");
                if (quiz != null && quizManager != null)
                {
                    quizManager.ShowQuiz(
                        quiz.question,
                        quiz.answers,
                        quiz.correctAnswerIndex,
                        () =>
                        {
                            subtitleManager.ShowObjective("Find an alternative escape route - try the window!");
                        }
                    );
                }
                else
                {
                    Debug.LogError("Quiz 'fire_blocked_door' not found or quizManager not assigned!");
                    subtitleManager.ShowObjective("Find an alternative escape route - try the window!");
                }
            }
        );

        Debug.Log("🔥 Door fire message shown - flag set");
    }

    // ✅ Public method for save system
    public static void RestoreFireMessageState(bool shown)
    {
        FireMessageShown = shown;
        Debug.Log($"🔥 Restored fire message state: shown={shown}");
    }

    // ✅ Reset on new game
    public static void ResetFireMessageProgress()
    {
        FireMessageShown = false;
        Debug.Log("🔥 Fire message progress reset");
    }

    public bool HasShownFireMessage()
    {
        return fireMessageShown;
    }
}