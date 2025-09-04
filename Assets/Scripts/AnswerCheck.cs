using TMPro;
using UnityEngine;

public class AnswerCheck : MonoBehaviour
{
    public TMP_Text testAnswerCheck;
    public int answerValue;                 // 0, 1, or 2 set per button in Inspector
    public QuizScript quizScript;           // assign or auto-find


    void Awake()
    {
        if (quizScript == null)
            quizScript = GetComponentInParent<QuizScript>();  // finds the one on the quiz panel
    }

    public void CheckAnswer()
    {
        if (quizScript == null)
        {
            Debug.LogError("AnswerCheck: QuizScript reference missing.");
            return;
        }

        testAnswerCheck.text = "Correct";
        if (answerValue == quizScript.correctAnswer) { }
        else{ testAnswerCheck.text = "Wrong"; }
}
    }

