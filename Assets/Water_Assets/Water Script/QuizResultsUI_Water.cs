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
        if (DataManager_Water.Instance == null)
        {
            Debug.LogWarning("[QuizResultsUI_Water] No DataManager instance found!");
            return;
        }

        int correct = DataManager_Water.Instance.quizScore;
        int wrong = DataManager_Water.Instance.wrongAnswers;
        int total = DataManager_Water.Instance.totalQuestionsAnswered; // ✅ fixed lowercase t

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
