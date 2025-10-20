using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloodQuizTrigger : MonoBehaviour
{
    [Header("Quiz Settings")]
    public FloodQuizSet quizSet;     // ScriptableObject holding all questions
    public FloodQuiz quizManager;    // Reference to your quiz UI manager
    public PlayerController_Water player;  // Reference to your player movement script

    private bool quizCompleted = false;
    private bool quizActive = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !quizCompleted && !quizActive)
        {
            quizActive = true;

            if (player == null)
                player = other.GetComponent<PlayerController_Water>();

            FreezePlayer();

            if (quizManager != null && quizSet != null)
            {
                // Remove old listener and add a new one
                quizManager.OnQuizComplete -= OnQuizAnswered;
                quizManager.OnQuizComplete += OnQuizAnswered;

                quizManager.BeginQuiz(quizSet.questions, true);
                Debug.Log($"🧩 Quiz triggered: {quizSet.name}");
            }
            else
            {
                Debug.LogWarning($"{name}: Missing QuizManager or QuizSet reference!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🚫 Player left trigger but quiz still active.");
        }
    }

    private void OnQuizAnswered(bool correct)
    {
        // Remove listener after answering
        quizManager.OnQuizComplete -= OnQuizAnswered;

        // Unfreeze player
        UnfreezePlayer();

        // Hide quiz UI
        if (quizManager != null)
            quizManager.HideQuiz();

        // Mark quiz as done
        quizCompleted = true;
        quizActive = false;

        // Log results to DataManager
        if (DataManager_Water.Instance != null)
        {
            if (correct)
            {
                DataManager_Water.Instance.AddQuizScore(1);
                Debug.Log($"✅ Correct answer! Total quiz score: {DataManager_Water.Instance.quizScore}");
            }
            else
            {
                DataManager_Water.Instance.AddWrongAnswer(1);
                Debug.Log($"❌ Wrong answer! Wrong answers: {DataManager_Water.Instance.wrongAnswers}");
            }

            int totalAnswered = DataManager_Water.Instance.quizScore + DataManager_Water.Instance.wrongAnswers;
            Debug.Log($"🧮 Total Questions Answered: {totalAnswered}");
        }

        // Disable trigger so quiz cannot be replayed
        GetComponent<Collider>().enabled = false;
    }

    // ----- Player Freeze/Unfreeze Methods -----
    private void FreezePlayer()
    {
        if (player == null) return;

        // Stop player input
        player.canMove = false;

        // Stop Rigidbody if used (just in case)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void UnfreezePlayer()
    {
        if (player == null) return;

        // Allow player input
        player.canMove = true;

        // Stop Rigidbody (safety)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }
}
