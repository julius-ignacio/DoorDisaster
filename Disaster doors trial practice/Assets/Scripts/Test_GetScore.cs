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
          scoreText.text = $"Points: {DataManager.Instance.quizScore}";
            quiestionsAnsweredText.text = $"Questions answered: {DataManager.Instance.totalQuestionsAnswered}";
        }
    }
}
