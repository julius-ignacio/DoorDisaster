using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizScript : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] choiceButtons;
    public TMP_Text scoreText;  

    private List<QuizQuestion> currentQuestions;
    private int currentIndex = 0;
        private int score = 0;       // ⬅️ Track correct answers

    public void BeginQuiz(List<QuizQuestion> questions)
    {
        currentQuestions = questions;
        currentIndex = 0;
         score = 0;
         UpdateScoreUI();
        ShowQuestion();
        gameObject.SetActive(true); // show panel
    }

    void ShowQuestion()
    {
        if (currentIndex >= currentQuestions.Count)
        {
            EndQuiz();
            return;
        }

        QuizQuestion q = currentQuestions[currentIndex];
        questionText.text = q.question;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            TMP_Text btnText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
            btnText.text = q.choices[i];

            // Remove any old listeners so clicks don’t stack
            choiceButtons[i].onClick.RemoveAllListeners();
        }
    }

    // 🔹 This is the method you’ll hook to the button OnClick()
    public void AnswerQuestion(int choiceIndex)
    {
        QuizQuestion q = currentQuestions[currentIndex];
        bool correct = (choiceIndex == q.correctIndex);

      if (correct)
        {
            score++;
            Debug.Log("✅ Correct!");
        }
        else
        {
            Debug.Log("❌ Wrong!");
        }

        UpdateScoreUI();
        currentIndex++;
        ShowQuestion();
    }

      void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}/{currentQuestions.Count}";
    }

    void EndQuiz()
    {
        Debug.Log("🎉 Quiz Finished!");
        gameObject.SetActive(false); // hide panel
    }
}
