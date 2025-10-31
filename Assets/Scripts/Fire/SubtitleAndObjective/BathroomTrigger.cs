using UnityEngine;

public class BathroomTrigger : MonoBehaviour
{
    public SubtitleManager2 subtitleManager;
    public FireSafetyQuiz quizManager;
    private bool hasTriggered = false;

    void Start()
    {
        // Debug checks
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("BathroomTrigger: No Collider attached!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("BathroomTrigger: Collider is not marked as Trigger!");
        }

        Debug.Log("BathroomTrigger: Ready and waiting for player");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"BathroomTrigger: Something entered! Tag: {other.tag}, Name: {other.name}");

        if (other.CompareTag("Player") && !hasTriggered)
        {
            Debug.Log("BathroomTrigger: Player detected! Triggering event...");
            hasTriggered = true;

            if (subtitleManager == null)
            {
                Debug.LogError("BathroomTrigger: SubtitleManager is not assigned!");
                return;
            }

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
        Debug.Log("BathroomTrigger: Showing quiz from database...");

        // Fetch quiz from database by ID
        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("wet_towel");

        if (quiz != null)
        {
            Debug.Log("BathroomTrigger: Quiz found, displaying...");

            if (quizManager == null)
            {
                Debug.LogError("BathroomTrigger: QuizManager is not assigned!");
                return;
            }

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

    // Visual debug in Scene view
    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;

            if (col is BoxCollider box)
                Gizmos.DrawCube(box.center, box.size);
            else if (col is SphereCollider sphere)
                Gizmos.DrawSphere(sphere.center, sphere.radius);
        }
    }
}