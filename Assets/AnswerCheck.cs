using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AnswerCheck : MonoBehaviour
{
    //public Button choice1, choice2, choice3;
    public TMP_Text testAnswerCheck;
    private QuizChoices quizData = new QuizChoices();
    private QuizScript quizScript = new QuizScript();
    public int answerValue;



    public void checkAnswer()
    {
        if (answerValue == quizScript.correctAnswer)
        {
            testAnswerCheck.text = "Correct";
        }
        else
        {
            testAnswerCheck.text = "Wrong";
        }
    }

}
