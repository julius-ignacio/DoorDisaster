using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MilkShake;

public class QuizScript : MonoBehaviour
{
    public GameObject helpBtn; // opens quiz panel
    public GameObject quizUI;  // quiz panel
    public GameObject hud;     // HUD root (ideally has a CanvasGroup)

    [Header("UI")]
    public TMP_Text questionText;
    public Button[] choiceButtons;   // assign in inspector (e.g. 3 buttons)
    public TMP_Text scoreText;       // shows player score

    [Header("Colors & Timing")]
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f); // green
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f);   // red
    public float feedbackDelay = 1f;

    [Header("Disable movements and quake when taking quiz")]
    public Movements movements;
    public ConsistentQuake consistentQuake;
    public Shaker shake;

    private List<QuizQuestion> currentQuestions;
    private int currentIndex = 0;
    public int score = 0;
    private Color[] originalButtonColors;

    public GameObject[] disablePlaneAfterQuiz;
    public int currentNpcId; // which NPC/item
    public ObjectBehaviorEvent[] objectBehaviorEvent;

    void Start()
    {
        if (quizUI != null) quizUI.SetActive(false);
        if (helpBtn != null) helpBtn.SetActive(false);
    }

    void Awake()
    {
        originalButtonColors = new Color[choiceButtons.Length];
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Image img = choiceButtons[i].GetComponent<Image>();
            originalButtonColors[i] = img != null ? img.color : Color.white;
        }
    }

    // Map your ids (1..15) to QuizGroup
    private QuizGroup ResolveGroup(int id)
    {
        switch (id)
        {
            case 1:  return QuizGroup.NPC1;
            case 2:  return QuizGroup.NPC2;
            case 3:  return QuizGroup.NPC3;
            case 4:  return QuizGroup.NPC4;
            case 5:  return QuizGroup.NPC5;

            case 6:  return QuizGroup.Medkit1;
            case 7:  return QuizGroup.Medkit2;
            case 8:  return QuizGroup.Medkit3;
            case 9:  return QuizGroup.Medkit4;

            case 10: return QuizGroup.Water1;
            case 11: return QuizGroup.Water2;
            case 12: return QuizGroup.Water3;
            case 13: return QuizGroup.Water4;

            case 14: return QuizGroup.Whistle1;
            case 15: return QuizGroup.SafetyHelmet;
            default:
                Debug.LogWarning($"Unknown currentNpcId {id}, defaulting to NPC1.");
                return QuizGroup.NPC1;
        }
    }

    public void OnHelpButtonClick()
    {
        if (helpBtn) helpBtn.SetActive(false);
        if (quizUI) quizUI.SetActive(true);

        // Read mode from DataManager and fetch the appropriate question set
        int mode = 0;
        if (DataManager.Instance != null)
            mode = Mathf.Clamp(DataManager.Instance.currentMode, 0, 1);

        var group = ResolveGroup(currentNpcId);
        List<QuizQuestion> questions = QuizDatabase.Get(group, mode);
        BeginQuiz(questions);
    }

    // Called when starting the quiz
    public void BeginQuiz(List<QuizQuestion> questions)
    {
        if (movements) movements.enabled = false;
        if (shake) shake.enabled = false;

        if (consistentQuake != null)
        {
            consistentQuake.PauseQuakes();
            consistentQuake.enabled = false;
        }

        var hudCanvas = hud ? hud.GetComponent<CanvasGroup>() : null;
        if (hudCanvas != null)
        {
            hudCanvas.alpha = 0;
            hudCanvas.interactable = false;
            hudCanvas.blocksRaycasts = false;
        }
        else if (hud != null)
        {
            hud.SetActive(false);
        }

        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("BeginQuiz called with empty question list.");
            return;
        }

        currentQuestions = new List<QuizQuestion>(questions);
        currentIndex = 0;
        score = 0;
        UpdateScoreUI();
        ShowQuestion();
        gameObject.SetActive(true);
    }

    void ShowQuestion()
    {
        if (currentIndex >= currentQuestions.Count)
        {
            EndQuiz(currentNpcId);
            return;
        }

        QuizQuestion q = currentQuestions[currentIndex];
        if (questionText) questionText.text = q.question;

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button btn = choiceButtons[i];

            if (i < q.choices.Length)
            {
                btn.gameObject.SetActive(true);

                TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = q.choices[i];

                Image img = btn.GetComponent<Image>();
                if (img != null) img.color = originalButtonColors[i];

                btn.interactable = true;
                btn.onClick.RemoveAllListeners();

                int choiceIndex = i; // capture
                btn.onClick.AddListener(() => OnChoiceButtonClicked(choiceIndex));
            }
            else
            {
                btn.gameObject.SetActive(false);
            }
        }

        Debug.Log($"Question: {q.question}, Choices={q.choices.Length}, Buttons={choiceButtons.Length}, CorrectIndex={q.correctIndex}");
    }

    public void OnChoiceButtonClicked(int choiceIndex)
    {
        if (currentQuestions == null || currentIndex >= currentQuestions.Count) return;
        if (choiceIndex < 0 || choiceIndex >= choiceButtons.Length)
        {
            Debug.LogError($"Choice index {choiceIndex} out of bounds for buttons length {choiceButtons.Length}");
            return;
        }

        QuizQuestion q = currentQuestions[currentIndex];
        if (choiceIndex >= q.choices.Length)
        {
            Debug.LogError($"Choice index {choiceIndex} out of bounds for choices length {q.choices.Length}");
            return;
        }

        bool correct = (choiceIndex == q.correctIndex);

        if (correct)
        {
            score++;
            if (DataManager.Instance != null) DataManager.Instance.quizScore++;
        }
        else
        {
            if (DataManager.Instance != null) DataManager.Instance.wrongAnswers++;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].interactable = false;

        Image chosenImg = choiceButtons[choiceIndex].GetComponent<Image>();
        if (chosenImg != null) chosenImg.color = correct ? correctColor : wrongColor;

        if (q.correctIndex >= 0 && q.correctIndex < choiceButtons.Length)
        {
            Image correctImg = choiceButtons[q.correctIndex].GetComponent<Image>();
            if (correctImg != null) correctImg.color = correctColor;
        }

        UpdateScoreUI();
        UpdateDataManager();

        StartCoroutine(AdvanceAfterDelay(feedbackDelay));
    }

    IEnumerator AdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentIndex++;
        ShowQuestion();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}/{currentQuestions?.Count ?? 0}";
    }

    void UpdateDataManager()
    {
        if (DataManager.Instance != null)
            DataManager.Instance.totalQuestionsAnswered++;
    }

    void EndQuiz(int idToDisableTriggers)
    {
        Debug.Log($"Quiz finished. NPC {currentNpcId}, Score={score}");

        if (DataManager.Instance != null)
            DataManager.Instance.npcScores[currentNpcId] = score;

        int idx = idToDisableTriggers - 1;
        if (disablePlaneAfterQuiz != null && idx >= 0 && idx < disablePlaneAfterQuiz.Length && disablePlaneAfterQuiz[idx] != null)
            disablePlaneAfterQuiz[idx].SetActive(false);

        if (objectBehaviorEvent != null && currentNpcId - 1 >= 0 &&
            currentNpcId - 1 < objectBehaviorEvent.Length && objectBehaviorEvent[currentNpcId - 1] != null)
        {
            objectBehaviorEvent[currentNpcId - 1].PlayAndDisappear(currentNpcId);
        }

        gameObject.SetActive(false);
        if (helpBtn) helpBtn.SetActive(false);

        if (consistentQuake != null)
        {
            consistentQuake.enabled = true;
            consistentQuake.ResumeQuakes();
        }

        var hudCanvas = hud ? hud.GetComponent<CanvasGroup>() : null;
        if (hudCanvas != null)
        {
            hudCanvas.alpha = 1;
            hudCanvas.interactable = true;
            hudCanvas.blocksRaycasts = true;
        }
        else if (hud != null)
        {
            hud.SetActive(true);
        }

        if (shake) shake.enabled = true;
        if (movements) movements.enabled = true;
    }
}