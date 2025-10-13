using UnityEngine;

public class DoorFireTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager subtitleManager;
<<<<<<< HEAD
    public CatPickup catPickup; // Reference to the cat pickup script
    public FireSafetyQuiz quizManager; // Use your FireSafetyQuiz that handles database
=======
    public FireSafetyQuiz quizManager;
    public ObjectiveManager objectiveManager;
>>>>>>> 47c3962 (Quiz script changes)

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
<<<<<<< HEAD
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
=======
            // If all essentials collected, show fire message
            else if (!fireMessageShown && objectiveManager != null && objectiveManager.GetObjectiveStage() >= 4)
            {
                TriggerFireSequence();
>>>>>>> 47c3962 (Quiz script changes)
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
<<<<<<< HEAD
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
=======
                if (!fireMessageShown)
                {
                    // Check if all essentials are collected (stage 4+)
                    if (objectiveManager != null && objectiveManager.GetObjectiveStage() >= 4)
                    {
                        TriggerFireSequence();
                    }
                    else
                    {
                        subtitleManager.ShowCustomMessage(
                            "I need to collect my essentials first!",
                            2f
                        );
                    }
                }
>>>>>>> 47c3962 (Quiz script changes)
            }
        }
    }

<<<<<<< HEAD
=======
    private void TriggerFireSequence()
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
                            // After quiz is done, objective stays pointing to window
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
    }

>>>>>>> 47c3962 (Quiz script changes)
    public bool HasShownFireMessage()
    {
        return fireMessageShown;
    }
}