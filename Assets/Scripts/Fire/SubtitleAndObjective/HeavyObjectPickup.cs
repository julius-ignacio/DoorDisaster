using UnityEngine;

public class HeavyObjectPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject heavyObject;
    public SubtitleManager2 subtitleManager;
    public WindowEscape windowEscapeScript;
    public FireSafetyQuiz quizManager;

    private bool hasPickedUp = false;
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Object");
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

    public void OnPickup()
    {
        if (!playerInRange || hasPickedUp) return;

        hasPickedUp = true;

        // Hide heavy object
        if (heavyObject != null)
            heavyObject.SetActive(false);

        // Hide pickup button
        GenericPickupButton.Instance.HidePickupPrompt();

        // Hide objective text
        subtitleManager.HideObjective();

        // Tell the window escape script player has the heavy object
        if (windowEscapeScript != null)
        {
            windowEscapeScript.PickupHeavyObject();
        }

        // Show message before quiz
        subtitleManager.ShowCustomMessage(
            "Got it! This should break the window!",
            2f,
            () =>
            {
                // Show the quiz
                QuizQuestion2 quiz = QuizDatabase2.GetQuiz("wb_window_trap");
                if (quiz != null && quizManager != null)
                {
                    quizManager.ShowQuiz(
                        quiz.question,
                        quiz.answers,
                        quiz.correctAnswerIndex,
                        () =>
                        {
                            // ✅ After quiz, remind player to break window and re-show button if in range
                            subtitleManager.ShowObjective("Use the heavy object to break the bedroom window");

                            if (windowEscapeScript != null && IsPlayerNearWindow())
                            {
                                GenericPickupButton.Instance.ShowPickupPrompt(windowEscapeScript, "Break Window");
                            }
                        }
                    );
                }
                else
                {
                    Debug.LogError("Quiz 'wb_window_trap' not found!");
                    subtitleManager.ShowObjective("Use the heavy object to break the bedroom window");

                    // Fallback: show button right away if near window
                    if (windowEscapeScript != null && IsPlayerNearWindow())
                    {
                        GenericPickupButton.Instance.ShowPickupPrompt(windowEscapeScript, "Break Window");
                    }
                }
            }
        );
    }

    private bool IsPlayerNearWindow()
    {
        if (windowEscapeScript == null) return false;

        Transform player = windowEscapeScript.player;
        return player != null && Vector3.Distance(player.position, windowEscapeScript.transform.position) < 3f;
    }

    public bool HasPickedUpObject()
    {
        return hasPickedUp;
    }
}
