using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SurveyManager : MonoBehaviour
{

    [Header("Tree")]
    public GameObject tree;
    public GameObject Light1, Light2;

    [Header("Barrier")]
    public GameObject Barrier;
    [Header("UI References")]
    public GameObject surveyUI;          // The panel that holds the survey UI
    public TMP_Text questionText;        // Text field for the question
    public Button[] choiceButtons;       // Buttons for answer choices
    public TMP_Text progressText;        // Optional: "Question 1 of X"

    [Header("Gameplay Control")]
    public GameObject hud;               // To disable HUD during survey
    public Movements movements;          // To disable player movement

    private List<SurveyQuestion> currentQuestions;
    private int currentIndex = 0;

    // Store player responses
    private List<int> selectedAnswers = new List<int>();

    void Start()
    {
        surveyUI.SetActive(false);

        if (DataManager.Instance.playerData.isSurveyDone == true)
        {
            tree.transform.localScale = new Vector3(16f, 20.07392f, 16f);
        }
        else
        {
            tree.transform.localScale = new Vector3(0.94f, 1.179f, 0.94f);

        }

        Light1.SetActive(false);
        Light2.SetActive(false);

    }

    void Update()
    {
        if (DataManager.Instance.playerData.isSurveyDone == true)
        {
            tree.transform.localScale = new Vector3(16f, 20.07392f, 16f);
        }
        else
        {
            tree.transform.localScale = new Vector3(0.94f, 1.179f, 0.94f);

        }

         Barrier.SetActive(!DataManager.Instance.playerData.isEarthFinished);

    }

    public void BeginSurvey(List<SurveyQuestion> questions)
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("Survey started with no questions.");
            return;
        }

        currentQuestions = new List<SurveyQuestion>(questions);
        currentIndex = 0;
        selectedAnswers.Clear();

        // Disable gameplay UI
        hud.SetActive(false);
        movements.enabled = false;

        surveyUI.SetActive(true);
        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (currentIndex >= currentQuestions.Count)
        {
            EndSurvey();
            return;
        }

        SurveyQuestion q = currentQuestions[currentIndex];
        questionText.text = q.question;
        progressText.text = $"Question {currentIndex + 1} of {currentQuestions.Count}";

        // Populate buttons
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button btn = choiceButtons[i];

            if (i < q.choices.Length)
            {
                btn.gameObject.SetActive(true);
                TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = q.choices[i];

                // Clear old listeners
                btn.onClick.RemoveAllListeners();

                int choiceIndex = i;
                btn.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    void OnChoiceSelected(int index)
    {
        selectedAnswers.Add(index);
        Debug.Log($"Answered Q{currentIndex + 1} with choice index {index}");
        currentIndex++;
        ShowQuestion();
    }

    void EndSurvey()
    {

        surveyUI.SetActive(false);
        hud.SetActive(true);
        movements.enabled = true;
        DataManager.Instance.playerData.isSurveyDone = true;
        GrowTree();
        Debug.Log("Survey completed!");
        Debug.Log("Responses:");
        for (int i = 0; i < selectedAnswers.Count; i++)
        {
            Debug.Log($"Q{i + 1}: Choice {selectedAnswers[i]}");
        }

        // Optional: Display a Thank You message
        // Or send data to DataManager / save system
    }

    void GrowTree()
    {
        if (DataManager.Instance.playerData.isSurveyDone)
        {
            StartCoroutine(GrowTreeSmoothly());
        }
    }

    IEnumerator GrowTreeSmoothly()
    {
        Vector3 startScale = tree.transform.localScale;
        Vector3 targetScale = new Vector3(16f, 20.07392f, 16f);
        float duration = 15f; // Adjust speed (seconds)
        float elapsed = 0f;

            Light1.SetActive(true);
        Light2.SetActive(true);
        
        AudioManager.Instance.PlaySFX(35);

        while (elapsed < duration)
        {
            tree.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        tree.transform.localScale = targetScale; // Ensure final scale
    }

}
