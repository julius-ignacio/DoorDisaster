using System;
using UnityEngine;

[System.Serializable]
public class QuizQuestion2
{
    public string questionID;
    [TextArea(3, 5)]
    public string question;
    public string[] answers;
    public int correctAnswerIndex;
}

public class QuizDatabase2 : MonoBehaviour
{
    [Header("All Quiz Questions")]
    public QuizQuestion2[] questions;

    private static QuizDatabase2 instance;

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

    // Get quiz by ID
    public static QuizQuestion2 GetQuiz(string questionID)
    {
        if (instance == null)
        {
            Debug.LogError("QuizDatabase instance not found!");
            return null;
        }

        foreach (QuizQuestion2 quiz in instance.questions)
        {
            if (quiz.questionID == questionID)
                return quiz;
        }

        Debug.LogError($"Quiz with ID '{questionID}' not found!");
        return null;
    }
}