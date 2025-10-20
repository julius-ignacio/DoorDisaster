using UnityEngine;

public class CabinetInteract_Water : MonoBehaviour, IInteractable_Water
{
    [Header("Quiz / Fact Settings (optional)")]
    public FloodQuiz floodQuiz;
    public FloodQuizSet quizSet;

    private Animator animator;
    private bool quizActive = false;

    private void Awake()
    {
        // Automatically find the Animator if not assigned
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning($"{name} has no Animator attached.");
    }

    public string GetPrompt()
    {
        return "Press E: Open Cabinet";
    }

    public void Interact()
    {
        Debug.Log($"{name} interacted with.");

        // 1️⃣ Play door open/close animation
        if (animator != null)
        {
            bool isOpen = animator.GetBool("open");
            animator.SetBool("open", !isOpen);
        }

        // 2️⃣ Trigger quiz if available
        if (!quizActive && floodQuiz != null && quizSet != null && quizSet.questions.Count > 0)
        {
            quizActive = true;
            floodQuiz.BeginQuiz(quizSet.questions);
            floodQuiz.OnQuizComplete += OnQuizComplete;
        }
    }

    private void OnQuizComplete(bool wasCorrect)
    {
        quizActive = false;
        floodQuiz.OnQuizComplete -= OnQuizComplete;

        if (wasCorrect)
            Debug.Log($"✅ Quiz completed correctly on {name}");
        else
            Debug.Log($"❌ Quiz failed on {name}");
    }
}
