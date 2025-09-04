using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HMStudio.EasyQuiz
{
    public class QuestionViewer : MonoBehaviour
    {
        [Header("Data")]
        public int questionID;
        public string questionText;
        public List<string> options = new List<string>();
        public string correctAnswer;

        [Header("UI Text References (TextMeshPro)")]
        [SerializeField] private TextMeshProUGUI _tmpQuestion;
        [SerializeField] private List<TextMeshProUGUI> _lstOptions = new List<TextMeshProUGUI>();

        private List<QuestionData> questions;

        public void SetQuestions(List<QuestionData> questions)
        {
            this.questions = questions;
        }

        public void LoadQuestionFromJson()
        {
            if (questions == null || questions.Count == 0 || questionID < 1 || questionID > questions.Count)
                return;
            var q = questions[questionID - 1];
            questionText = q.question;
            options = q.options;
            correctAnswer = q.correct;
            UpdateTextFields();
        }

        public void UpdateTextFields()
        {
            if (_tmpQuestion != null)
                _tmpQuestion.text = questionText;
            for (int i = 0; i < _lstOptions.Count && i < options.Count; i++)
                _lstOptions[i].text = options[i];
        }
    }
}