using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("Score Tracking")]
    public int quizScore = 0;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddQuizScore(int points)
    {
        quizScore += points;
        Debug.Log("Quiz Score: " + quizScore);
    }

    public int GetTotalScore()
    {
        return quizScore;
    }

    public void ResetScore()
    {
        quizScore = 0;
    }
}