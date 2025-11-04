using TMPro;
using UnityEngine;

public class Test_GetScore : MonoBehaviour
{
    public TMP_Text scoreText;

    void Update()
    {
        if (DataManager.Instance != null)
        {
          scoreText.text = $"Points: {DataManager.Instance.quizScore}";
        }
    }
}
