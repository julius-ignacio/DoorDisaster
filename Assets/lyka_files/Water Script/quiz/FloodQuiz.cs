using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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

    [Header("Flood Time Settings")]
    public WaterRising waterRising;   // ✅ Drag your WaterRising script here
    public float timePenalty = 5f;    // ✅ Seconds to subtract for wrong answers
    public float timeReward = 5f;     // ✅ Seconds to add for correct answers

    [Header("Floating Text UI")]
    public TMP_Text penaltyText;      // ✅ Drag your red “±time” TMP text here

    [Header("Feedback Settings")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

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

        if (penaltyText != null)
            penaltyText.alpha = 0f; // hide initially
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

        Image clickedImage = choiceObjects[index].GetComponent<Image>();
        if (clickedImage != null)
            clickedImage.color = correct ? Color.green : Color.red;

        if (audioSource != null)
            audioSource.PlayOneShot(correct ? correctSound : wrongSound);

        if (!correct)
        {
            // Highlight correct answer
            if (q.correctIndex >= 0 && q.correctIndex < choiceObjects.Length)
            {
                Image correctImage = choiceObjects[q.correctIndex].GetComponent<Image>();
                if (correctImage != null)
                    correctImage.color = Color.green;
            }

            // Apply penalty
            if (waterRising != null)
            {
                waterRising.ApplyPenaltyFromQuiz(timePenalty);
                Debug.Log($"❌ Wrong answer! -{timePenalty}s from timer.");
            }

            if (penaltyText != null)
                StartCoroutine(ShowFloatingTime(-timePenalty, Color.red));

            Invoke(nameof(WrongAnswer), 1.5f);
        }
        else
        {
            // Apply reward ✅
            if (waterRising != null)
            {
                waterRising.ApplyRewardFromQuiz(timeReward);
                Debug.Log($"✅ Correct answer! +{timeReward}s added to timer.");
            }

            if (penaltyText != null)
                StartCoroutine(ShowFloatingTime(timeReward, Color.green));

            Invoke(nameof(CorrectAnswer), 1.2f);
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

    // 🆕 Floating “±time” display that stays visible
    private IEnumerator ShowFloatingTime(float seconds, Color color)
    {
        penaltyText.gameObject.SetActive(true);
        penaltyText.text = (seconds > 0 ? "+" : "") + $"{seconds:F0}s";
        penaltyText.color = color;
        penaltyText.alpha = 1f;

        // Optional: gentle upward float (remove if you want it static)
        Vector3 startPos = penaltyText.rectTransform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 20f, 0);
        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            penaltyText.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            yield return null;
        }

        // ✅ Keep text visible — no fade-out, no reset
        penaltyText.rectTransform.localPosition = endPos;
        penaltyText.alpha = 1f;
    }
}
