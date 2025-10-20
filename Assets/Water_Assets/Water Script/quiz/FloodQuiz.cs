using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FloodQuiz : MonoBehaviour
{
    [Header("UI References")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public GameObject[] choiceObjects;

    private TextMeshProUGUI[] choiceTexts;
    private List<FloodQuestion> currentQuestions;
    private int currentQuestionIndex;

    private bool isLocked = false;

    public event System.Action<bool> OnQuizComplete;

    void Awake()
    {
        choiceTexts = new TextMeshProUGUI[choiceObjects.Length];

        for (int i = 0; i < choiceObjects.Length; i++)
        {
            choiceTexts[i] = choiceObjects[i].GetComponentInChildren<TextMeshProUGUI>();
            int index = i;

            Button btn = choiceObjects[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnChoiceSelected(index));
            }
        }

        if (quizPanel != null)
            quizPanel.SetActive(false);
    }

    public void BeginQuiz(List<FloodQuestion> questions, bool resetColors = true)
    {
        if (quizPanel == null || questions == null || questions.Count == 0)
            return;

        isLocked = false;
        ResetButtonColors();

        quizPanel.SetActive(true);
        currentQuestions = questions;
        currentQuestionIndex = Random.Range(0, currentQuestions.Count);
        ShowQuestion();
    }

    void ShowQuestion()
    {
        FloodQuestion q = currentQuestions[currentQuestionIndex];
        questionText.text = q.question;

        for (int i = 0; i < choiceObjects.Length; i++)
        {
            choiceObjects[i].SetActive(i < q.choices.Length);
            if (i < q.choices.Length)
                choiceTexts[i].text = q.choices[i];
        }
    }

    void OnChoiceSelected(int index)
    {
        if (isLocked) return;
        isLocked = true;

        FloodQuestion q = currentQuestions[currentQuestionIndex];
        bool correct = index == q.correctIndex;

        // Feedback colors
        Image btnImage = choiceObjects[index].GetComponent<Image>();
        if (btnImage != null)
            btnImage.color = correct ? Color.green : Color.red;

        // ✅ Correct: delay then close
        if (correct)
        {
            Invoke(nameof(CorrectAnswer), 1.2f);
        }
        else
        {
            // ❌ Wrong: flash red then close immediately
            Invoke(nameof(WrongAnswer), 0.8f);
        }
    }

    void CorrectAnswer()
    {
        OnQuizComplete?.Invoke(true);
        HideQuiz();
    }

    void WrongAnswer()
    {
        OnQuizComplete?.Invoke(false);
        HideQuiz();
    }

    public void HideQuiz()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);
        isLocked = false;
    }

    void ResetButtonColors()
    {
        foreach (var obj in choiceObjects)
        {
            if (obj == null) continue;
            Image img = obj.GetComponent<Image>();
            if (img != null) img.color = Color.white;
        }
    }
}
