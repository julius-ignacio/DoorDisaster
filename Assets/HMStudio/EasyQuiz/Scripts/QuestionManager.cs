using System;
using System.IO;
using UnityEngine;

namespace HMStudio.EasyQuiz
{
    public class QuestionManager : MonoBehaviour
    {
        [Header("Reference")]
        [Tooltip("Reference to the QuestionViewer in the scene")]
        public QuestionViewer questionViewer;

        // Variable to count the number of correct answers
        private int correctAnswersCount = 0;

        /// <summary>
        /// Selects an answer for the current question.
        /// Compares the answer with the correctAnswer of the QuestionViewer (case-insensitive).
        /// If the answer is correct, increments the number of correct answers.
        /// Then, automatically proceeds to the next question.
        /// Returns true if the answer is correct, false otherwise.
        /// </summary>
        /// <param name="answer">The selected answer</param>
        /// <returns>true if correct, false if incorrect</returns>
        public bool AnswerQuestion(string answer)
        {
            bool isCorrect = string.Equals(answer, questionViewer.correctAnswer, StringComparison.OrdinalIgnoreCase);
            if (isCorrect)
            {
                correctAnswersCount++;
            }
            return isCorrect;
        }

        /// <summary>
        /// Proceeds to the next question.
        /// If it exceeds the total number of questions, returns to the first question.
        /// Then loads the data of the new question.
        /// </summary>
        public void NextQuestion()
        {
            int total = GetTotalQuestions();
            questionViewer.questionID++;
            if (questionViewer.questionID > total)
                questionViewer.questionID = 1;
            questionViewer.LoadQuestionFromExcel();
        }

        /// <summary>
        /// Proceeds to the previous question.
        /// If it goes below 1, resets to the total number of questions.
        /// Then loads the data of the new question.
        /// </summary>
        public void PrevQuestion()
        {
            int total = GetTotalQuestions();
            questionViewer.questionID--;
            if (questionViewer.questionID < 1)
                questionViewer.questionID = total;
            questionViewer.LoadQuestionFromExcel();
        }

        /// <summary>
        /// Returns the current score (number of correct answers).
        /// </summary>
        /// <returns>The score as a float</returns>
        public float GetPoint()
        {
            return (float)correctAnswersCount / GetTotalQuestions();
        }

        /// <summary>
        /// Gets the current question information in the format: "Question {questionID} / {totalQuestions}"
        /// </summary>
        /// <returns>The information string</returns>
        public string GetInfo()
        {
            int total = GetTotalQuestions();
            return $"Question {questionViewer.questionID} / {total}";
        }

        /// <summary>
        /// Gets the statistics in the format "number of correct answers / total number of questions".
        /// </summary>
        /// <returns>The statistics string</returns>
        public string GetStatistic()
        {
            int total = GetTotalQuestions();
            return $"Correct {correctAnswersCount} / {total}";
        }

        /// <summary>
        /// Reads the Excel file to count the total number of questions.
        /// Assumes the Excel file has a header in row 0 and data starting from row 1.
        /// </summary>
        /// <returns>The total number of questions</returns>
        public int GetTotalQuestions()
        {
            return questionViewer.GetTotalQuestions();
        }






















public void LoadQuizFile(string fileName)
{
    string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

    // Fix for when Unity returns "file://..." on some platforms
    if (filePath.StartsWith("file://"))
        filePath = filePath.Replace("file://", "");

    questionViewer.SetExcelFile(filePath);
}


    }
}