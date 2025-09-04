using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuizScript : MonoBehaviour
{
    public GameObject choice1, choice2, choice3;
    public int selected, correctAnswer;
    public Camera npcCamera;
    public TMP_Text questionText;
    private QuizChoices quizData = new QuizChoices();


    public void Start()
    {
        npcCamera.enabled = false;
    }
    public void ChangeTexts()
    {
        // Safety check
        if (selected < 0 || selected >= quizData.questions.Count) return;

        // Get the selected question
        QuizQuestion q = quizData.questions[selected];

        // Update question
        questionText.text = q.question;

        // Update choices
        choice1.GetComponentInChildren<TMP_Text>().text = q.choices[0];
        choice2.GetComponentInChildren<TMP_Text>().text = q.choices[1];
        choice3.GetComponentInChildren<TMP_Text>().text = q.choices[2];

        correctAnswer = q.correctIndex;
    }


    public void QuizDone()
    {
        // Hide quiz UI
        gameObject.SetActive(false);


    }
}
