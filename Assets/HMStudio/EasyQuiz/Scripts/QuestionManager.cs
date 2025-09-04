using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HMStudio.EasyQuiz
{
    public class QuestionManager : MonoBehaviour
    {
        [Header("Reference")]
        public QuestionViewer questionViewer;

        private int correctAnswersCount = 0;
        public List<QuestionData> questions = new List<QuestionData>();

        public void LoadQuizFile(string fileName)
        {
            StartCoroutine(LoadQuizJsonCoroutine(fileName));
        }

        private IEnumerator LoadQuizJsonCoroutine(string fileName)
        {
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                questions = JsonUtility.FromJson<QuestionListWrapper>("{\"questions\":" + json + "}").questions;
                questionViewer.SetQuestions(questions);
                questionViewer.questionID = 1;
                questionViewer.LoadQuestionFromJson();
            }
            else
            {
                Debug.LogError("Failed to load quiz JSON: " + www.error);
            }
        }

        public bool AnswerQuestion(string answer)
        {
            bool isCorrect = string.Equals(answer, questionViewer.correctAnswer, System.StringComparison.OrdinalIgnoreCase);
            if (isCorrect)
            {
                correctAnswersCount++;
            }
            return isCorrect;
        }

        public void NextQuestion()
        {
            int total = GetTotalQuestions();
            questionViewer.questionID++;
            if (questionViewer.questionID > total)
                questionViewer.questionID = 1;
            questionViewer.LoadQuestionFromJson();
        }

        public int GetTotalQuestions()
        {
            return questions.Count;
        }
    }
}