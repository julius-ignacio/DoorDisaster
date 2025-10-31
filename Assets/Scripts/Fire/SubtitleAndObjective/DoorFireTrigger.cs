using UnityEngine;

public class DoorFireTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public FireSafetyQuiz quizManager;
    public ObjectiveManager objectiveManager;

    private bool fireMessageShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Only proceed if essentials are collected
            if (!fireMessageShown && objectiveManager != null && objectiveManager.GetObjectiveStage() >= 4)
            {
                TriggerFireSequence();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Only trigger after essentials are collected
                if (!fireMessageShown)
                {
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
            }
        }
    }

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
                QuizQuestion2 quiz = QuizDatabase2.GetQuiz("fire_blocked_door");
                if (quiz != null && quizManager != null)
                {
                    quizManager.ShowQuiz(
                        quiz.question,
                        quiz.answers,
                        quiz.correctAnswerIndex,
                        () =>
                        {
                            // After quiz is done, keep the objective visible
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

    public bool HasShownFireMessage()
    {
        return fireMessageShown;
    }
}
