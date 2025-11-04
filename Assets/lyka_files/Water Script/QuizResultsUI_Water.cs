using TMPro;
using UnityEngine;

public class QuizResultsUI_Water : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text totalText;

    private void OnEnable()
    {
        UpdateResults();
    }

    public void UpdateResults()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogWarning("[QuizResultsUI_Water] No DataManager instance found!");
            return;
        }

        int correct = DataManager.Instance.quizScore;
        int wrong = DataManager.Instance.wrongAnswers;
        int total = correct + wrong; // ✅ total questions = correct + wrong

        if (correctText != null)
            correctText.text = $"Correct Answers: {correct}";
        else
            Debug.LogWarning("[QuizResultsUI_Water] Missing reference: correctText");

        if (wrongText != null)
            wrongText.text = $"Wrong Answers: {wrong}";
        else
            Debug.LogWarning("[QuizResultsUI_Water] Missing reference: wrongText");

        if (totalText != null)
            totalText.text = $"Total Questions: {total}";
        else
            Debug.LogWarning("[QuizResultsUI_Water] Missing reference: totalText");
    }

    private void Update()
    {
        // 🔁 Optional live update if you want the results to refresh every frame
        UpdateResults();
    }
}
