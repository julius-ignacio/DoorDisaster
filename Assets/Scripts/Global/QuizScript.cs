using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizScript : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text questionText;
    public Button[] choiceButtons;            // assign in inspector (e.g. 3 buttons)
    public TMP_Text scoreText;               // shows player score
    public TMP_Text Scoree;                  // result text you used wala lang to pang test langggg ignore this

    [Header("Colors & Timing")] //Ignore these
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f); // green
    public Color wrongColor   = new Color(0.9f, 0.3f, 0.3f); // red
    public float feedbackDelay = 2f;                       // seconds to show color. Delay to each after quiz question


    private List<QuizQuestion> currentQuestions; // Rereference to current quiz questions
    private int currentIndex = 0; // which question we are on
    private int score = 0; // how many correct so far... 
    private Color[] originalButtonColors; //ignore

    void Awake()
    {
        // cache original button background colors so we can restore them..     ignoreeeeeeee
        originalButtonColors = new Color[choiceButtons.Length];
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Image img = choiceButtons[i].GetComponent<Image>();
            originalButtonColors[i] = img != null ? img.color : Color.white;
        }
    }


//Is called when the player clicks the BUTTON in the other script 
    public void BeginQuiz(List<QuizQuestion> questions)
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("BeginQuiz called with empty question list.");
            return;
        }

//Set up. passing the questions from the other script
        currentQuestions = new List<QuizQuestion>(questions);
        currentIndex = 0;
        score = 0;
        UpdateScoreUI(); //Update methods
        ShowQuestion();
        gameObject.SetActive(true);
    }

    void ShowQuestion()  //can just copy paste this method
    {
        // finished?
        if (currentIndex >= currentQuestions.Count)
        {
            EndQuiz();
            return;
        }

        QuizQuestion q = currentQuestions[currentIndex];

        // show text
        questionText.text = q.question;

        // populate buttons, remove old listeners, reset colors and interactability
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button btn = choiceButtons[i];

            // If question has fewer choices than number of buttons: hide extras
            if (i < q.choices.Length)
            {
                btn.gameObject.SetActive(true);
                TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
                if (btnText != null) btnText.text = q.choices[i];

                // reset color
                Image img = btn.GetComponent<Image>();
                if (img != null) img.color = originalButtonColors[i];

                // reset interactivity
                btn.interactable = true;

                // remove previous listeners then add new one
                btn.onClick.RemoveAllListeners();
                int choiceIndex = i; // capture
                btn.onClick.AddListener(() => OnChoiceButtonClicked(choiceIndex));
            }
            else
            {
                // hide unused buttons (if any)
                btn.gameObject.SetActive(false);
            }
        }
    }

    // central handler for button clicks
    public void OnChoiceButtonClicked(int choiceIndex)
    {
        // safety checks
        if (currentQuestions == null || currentIndex >= currentQuestions.Count) return;

        QuizQuestion q = currentQuestions[currentIndex];

        bool correct = (choiceIndex == q.correctIndex);
        if (correct) score++;

        // show colors: chosen button red/green; also show correct button as green
        // disable all buttons so user can't click multiple times
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button b = choiceButtons[i];
            b.interactable = false;
        }

        // color chosen
        Image chosenImg = choiceButtons[choiceIndex].GetComponent<Image>();
        if (chosenImg != null) chosenImg.color = correct ? correctColor : wrongColor;

        // color correct answer (if different)
        if (q.correctIndex >= 0 && q.correctIndex < choiceButtons.Length)
        {
            Image correctImg = choiceButtons[q.correctIndex].GetComponent<Image>();
            if (correctImg != null) correctImg.color = correct ? correctColor : correctColor;
        }

        UpdateScoreUI();

        // advance after a short delay
        StartCoroutine(AdvanceAfterDelay(feedbackDelay));
    }

    IEnumerator AdvanceAfterDelay(float delay)
    {
        // Wait scaled time. If you pause the game with Time.timeScale = 0 during quizzes,
        // change this to WaitForSecondsRealtime(delay).
        yield return new WaitForSeconds(delay);

        currentIndex++;
        ShowQuestion();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}/{currentQuestions?.Count ?? 0}";
        if (Scoree != null) Scoree.text = $"Score: {score}/{currentQuestions?.Count ?? 0}";
    }

    void EndQuiz()
    {
        Debug.Log("Quiz finished. Final score: " + score);
        // show result UI here or fire event
        gameObject.SetActive(false);
    }
}
