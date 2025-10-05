using System;
using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    public string questionID; // Unique identifier like "wet_towel", "stop_drop_roll", etc.
    [TextArea(3, 5)]
    public string question;
    public string[] answers;
    public int correctAnswerIndex;
}

public class QuizDatabase : MonoBehaviour
{
    [Header("All Quiz Questions")]
    public QuizQuestion[] questions;

    private static QuizDatabase instance;

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
    public static QuizQuestion GetQuiz(string questionID)
    {
        if (instance == null)
        {
            Debug.LogError("QuizDatabase instance not found!");
            return null;
        }

        foreach (QuizQuestion quiz in instance.questions)
        {
            if (quiz.questionID == questionID)
                return quiz;
        }

        Debug.LogError($"Quiz with ID '{questionID}' not found!");
        return null;
    }
}