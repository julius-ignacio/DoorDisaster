using TMPro;
using UnityEngine;

public class Test_GetScore : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text quiestionsAnsweredText;

    void Update()
    {
        if (DataManager.Instance != null)
        {
          scoreText.text = $"correcttt: {StageData.Instance.quizScore}";
            quiestionsAnsweredText.text = $"Total questions: {StageData.Instance.questionsAnswered}";
        }
    }
}
