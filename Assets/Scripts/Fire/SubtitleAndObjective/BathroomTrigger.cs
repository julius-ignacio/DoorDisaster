using UnityEngine;

public class BathroomTrigger : MonoBehaviour
{
    public SubtitleManager2 subtitleManager;
    public FireSafetyQuiz quizManager;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            subtitleManager.ShowCustomMessage(
                "It's hard to breathe with all this smoke! I need a wet towel to cover my face!",
                3f,
                () => {
                    // After subtitle ends, show the quiz using database
                    ShowQuizFromDatabase();
                }
            );
        }
    }

    void ShowQuizFromDatabase()
    {
        // Fetch quiz from database by ID
        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("wet_towel");

        if (quiz != null)
        {
            // Show quiz, and when complete, show the objective
            quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => {
                subtitleManager.HideObjective();
                subtitleManager.ShowObjective("Find a wet towel in the bathroom");
            });
        }
        else
        {
            Debug.LogError("Quiz 'wet_towel' not found in database!");
        }
    }
}