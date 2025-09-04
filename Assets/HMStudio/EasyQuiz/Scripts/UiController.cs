using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HMStudio.EasyQuiz
{
    public class UiController : MonoBehaviour
    {
        [SerializeField] private QuestionManager _questionManager;
        [Space] 
       
        
        [Space]
        [Header("Options")]
        [SerializeField] private Button _btnOptionA;
        [SerializeField] private Button _btnOptionB;
        [SerializeField] private Button _btnOptionC;
        [SerializeField] private Button _btnOptionD;
        
        private void Awake()
        {
            AnswerQuestion(_btnOptionA);
            AnswerQuestion(_btnOptionB);
            AnswerQuestion(_btnOptionC);
            AnswerQuestion(_btnOptionD);
        }

        private void AnswerQuestion(Button btnOption)
        {
            btnOption.onClick.AddListener(() =>
            {
                var tmps = btnOption.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var tmp in tmps)
                {
                    if (tmp.gameObject.name.Contains("tmpAnswer"))
                    {
                        var answerOption = tmp.text.Trim();
                        var isCorrect = _questionManager.AnswerQuestion(answerOption);
                        if (isCorrect)
                        {
                            Debug.LogWarning($"Answer {answerOption} is CORRECT");
                        }
                        else
                        {
                            Debug.LogWarning($"Answer {answerOption} is not CORRECT");
                        }
                    }
                }
                NextQuestion();
            });
        }

        private void Start()
        {
            _questionManager.questionViewer.questionID = 0;
            NextQuestion();
        }

        private void NextQuestion()
        {
            _questionManager.NextQuestion();
        }

    }
}