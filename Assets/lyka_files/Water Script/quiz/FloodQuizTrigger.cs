using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloodQuizTrigger : MonoBehaviour
{
    [Header("Quiz Settings")]
    public FloodQuizSet quizSet;
    public FloodQuiz quizManager;
    public PlayerController_Water player;

    private bool quizCompleted = false;
    private bool quizActive = false;
    private Collider triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        // Hide the trigger at the start (but keep GameObject active)
        triggerCollider.enabled = false;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = false;

        Debug.Log($"🟣 {name}: Hidden and disabled at start.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (quizCompleted || quizActive) return;

        quizActive = true;

        if (player == null)
            player = other.GetComponent<PlayerController_Water>();

        FreezePlayer();

        if (quizManager != null && quizSet != null)
        {
            quizManager.OnQuizComplete -= OnQuizAnswered;
            quizManager.OnQuizComplete += OnQuizAnswered;

            quizManager.BeginQuiz(quizSet.questions, true);
            Debug.Log($"🧩 Quiz triggered: {quizSet.name}");
        }
        else
        {
            Debug.LogWarning($"{name}: Missing QuizManager or QuizSet reference!");
            UnfreezePlayer();
            quizActive = false;
        }
    }

    private void OnQuizAnswered(bool correct)
    {
        quizManager.OnQuizComplete -= OnQuizAnswered;
        UnfreezePlayer();

        if (quizManager != null)
            quizManager.HideQuiz();

        quizCompleted = true;
        quizActive = false;

        if (DataManager.Instance != null)
        {
            if (correct)
            {
                DataManager.Instance.quizScore++;
            }
            else
            {
                DataManager.Instance.wrongAnswers++;
            }

        }

        triggerCollider.enabled = false;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = false;

        Debug.Log($"📕 {name}: Quiz finished and trigger disabled.");
    }

    // ✅ Called by Breaker when it's turned OFF
    public void ActivateTrigger()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        triggerCollider.enabled = true;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.enabled = true;

        Debug.Log($"📘 Quiz Trigger Activated: {name}");
    }

    // ------------------ Player Freeze / Unfreeze ------------------
    private void FreezePlayer()
    {
        if (player == null) return;
        player.canMove = false;



        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void UnfreezePlayer()
    {
        if (player == null) return;
        player.canMove = true;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }
}
