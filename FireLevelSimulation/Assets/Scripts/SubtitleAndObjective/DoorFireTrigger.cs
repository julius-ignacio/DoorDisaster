using UnityEngine;

public class DoorFireTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager subtitleManager;
    public CatPickup catPickup; // Reference to the cat pickup script
    public FireSafetyQuiz quizManager; // Use your FireSafetyQuiz that handles database

    private bool shockShown = false;
    private bool fireMessageShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Show shock reaction first time
            if (!shockShown)
            {
                shockShown = true;
                subtitleManager.HideObjective();
                subtitleManager.ShowCustomMessage("!!", 1.5f);
            }
            // If cat is rescued and fire message hasn't been shown, show it automatically
            else if (catPickup != null && catPickup.HasPickedUpCat() && !fireMessageShown)
            {
                fireMessageShown = true;
                subtitleManager.HideObjective();
                subtitleManager.ShowCustomMessage(
                    "The door is blocked by fire!",
                    2.5f,
                    () =>
                    {
                        subtitleManager.ShowObjective("Find an alternative escape route - try the window!");

                        // Fetch quiz from database
                        QuizQuestion quiz = QuizDatabase.GetQuiz("fire_blocked_door");
                        if (quiz != null && quizManager != null)
                        {
                            quizManager.ShowQuiz(
                                quiz.question,
                                quiz.answers,
                                quiz.correctAnswerIndex,
                                () =>
                                {
                                    // After quiz is done, you can continue the game
                                    subtitleManager.ShowObjective("Find an alternative escape route - try the window!");
                                }
                            );
                        }
                        else
                        {
                            Debug.LogError("Quiz 'fire_blocked_door' not found or quizManager not assigned!");
                        }
                    }
                );
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (catPickup != null && catPickup.HasPickedUpCat())
                {
                    if (!fireMessageShown)
                    {
                        fireMessageShown = true;
                        subtitleManager.ShowCustomMessage(
                            "The door is blocked by fire!",
                            2.5f,
                            () =>
                            {
                                subtitleManager.ShowObjective("Find an alternative escape route - try the window!");

                                // Fetch quiz from database
                                QuizQuestion quiz = QuizDatabase.GetQuiz("fire_blocked_door");
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
                                }
                            }
                        );
                    }
                }
                else
                {
                    subtitleManager.ShowCustomMessage("I need to save the cat first before I can escape!", 2f);
                }
            }
        }
    }

    public bool HasShownFireMessage()
    {
        return fireMessageShown;
    }
}