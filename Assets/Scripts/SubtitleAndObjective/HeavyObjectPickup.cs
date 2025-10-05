using UnityEngine;

public class HeavyObjectPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject heavyObject; // The chair/lamp GameObject
    public SubtitleManager subtitleManager;
    public WindowEscape windowEscapeScript; // Reference to the window escape script
    public FireSafetyQuiz quizManager; // Reference to the quiz system

    private bool hasPickedUp = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            if (Input.GetKey(KeyCode.E))
            {
                hasPickedUp = true;

                // Hide heavy object
                if (heavyObject != null)
                    heavyObject.SetActive(false);

                // Hide objective
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
                        // Show the quiz "window_blocked"
                        QuizQuestion quiz = QuizDatabase.GetQuiz("wb_window_trap");
                        if (quiz != null && quizManager != null)
                        {
                            quizManager.ShowQuiz(
                                quiz.question,
                                quiz.answers,
                                quiz.correctAnswerIndex,
                                () => subtitleManager.ShowObjective("Use the heavy object to break the bedroom window")
                            );
                        }
                        else
                        {
                            Debug.LogError("Quiz 'window_blocked' not found!");
                            subtitleManager.ShowObjective("Use the heavy object to break the bedroom window");
                        }
                    }
                );
            }
        }
    }

    public bool HasPickedUpObject()
    {
        return hasPickedUp;
    }
}
