using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MilkShake;

public class QuizScript_water : MonoBehaviour
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

    private List<QuizQuestion_water> currentQuestions;
    private int currentIndex = 0;
    public int score = 0;
    private Color[] originalButtonColors;

    public GameObject[] disableObjectAfterQuiz;
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
    private QuizGroup_water ResolveGroup(int id)
    {
        switch (id)
        {
            case 1: return QuizGroup_water.NPC1;
            case 2: return QuizGroup_water.NPC2;
            case 3: return QuizGroup_water.NPC3;
            case 4: return QuizGroup_water.NPC4;
            case 5: return QuizGroup_water.NPC5;

            case 6: return QuizGroup_water.Medkit1;
            case 7: return QuizGroup_water.Medkit2;
            case 8: return QuizGroup_water.Medkit3;
            case 9: return QuizGroup_water.Medkit4;

            case 10: return QuizGroup_water.Water1;
            case 11: return QuizGroup_water.Water2;
            case 12: return QuizGroup_water.Water3;
            case 13: return QuizGroup_water.Water4;

            case 14: return QuizGroup_water.Whistle1;
            case 15: return QuizGroup_water.SafetyHelmet;
            default:
                Debug.LogWarning($"Unknown currentNpcId {id}, defaulting to NPC1.");
                return QuizGroup_water.NPC1;
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
        List<QuizQuestion_water> questions = QuizDatabase_water.Get(group, mode);
        BeginQuiz(questions);
    }

    // Called when starting the quiz
    public void BeginQuiz(List<QuizQuestion_water> questions)
    {
        if (movements) movements.enabled = false;

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

        currentQuestions = new List<QuizQuestion_water>(questions);
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

        QuizQuestion_water q = currentQuestions[currentIndex];
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

        QuizQuestion_water q = currentQuestions[currentIndex];
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

// Replace only the EndQuiz method in your QuizScript_water.cs with the code below.

void EndQuiz(int idToDisableTriggers)
{
    Debug.Log($"Quiz finished. NPC {currentNpcId}, Score={score}");

    if (DataManager.Instance != null)
        DataManager.Instance.npcScores[currentNpcId] = score;

    int idx = idToDisableTriggers - 1;

    // 1) Invoke the object's PlayAndDisappear first while the object is still active.
    if (objectBehaviorEvent != null && currentNpcId - 1 >= 0 &&
        currentNpcId - 1 < objectBehaviorEvent.Length && objectBehaviorEvent[currentNpcId - 1] != null)
    {
        var obe = objectBehaviorEvent[currentNpcId - 1];
        try
        {
            // Prefer water-specific method if present; otherwise call the normal one.
            var methodWater = obe.GetType().GetMethod("PlayAndDisappear_water");
            if (methodWater != null)
                methodWater.Invoke(obe, new object[] { currentNpcId });
            else
            {
                var method = obe.GetType().GetMethod("PlayAndDisappear");
                if (method != null)
                    method.Invoke(obe, new object[] { currentNpcId });
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"EndQuiz: error invoking PlayAndDisappear variant: {e.Message}");
        }
    }

    // 2) Now disable the trigger/plane/other object AFTER we've invoked PlayAndDisappear.
    //    Use a short delay so the target object's coroutine can be started successfully.
    if (disableObjectAfterQuiz != null && idx >= 0 && idx < disableObjectAfterQuiz.Length && disableObjectAfterQuiz[idx] != null)
    {
        // If disableObjectAfterQuiz[idx] is the SAME GameObject that has the ObjectBehaviorEvent,
        // you can skip disabling here because PlayAndDisappear will hide the npcModel later.
        // Otherwise, schedule a short delayed disable so StartCoroutine has a chance to run.
        StartCoroutine(DisableObjectAfterDelay(idx, 0.05f));
    }

    gameObject.SetActive(false);
    if (helpBtn) helpBtn.SetActive(false);

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

    if (movements) movements.enabled = true;
}

private IEnumerator DisableObjectAfterDelay(int idx, float delay)
{
    yield return new WaitForSeconds(delay);
    if (disableObjectAfterQuiz != null && idx >= 0 && idx < disableObjectAfterQuiz.Length && disableObjectAfterQuiz[idx] != null)
        disableObjectAfterQuiz[idx].SetActive(false);
}
}