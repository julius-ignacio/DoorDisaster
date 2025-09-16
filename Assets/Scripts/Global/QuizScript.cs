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
   // public TMP_Text Scoree;                  // result text you used wala lang to pang test langggg ignore this

    [Header("Colors & Timing")] //Ignore these
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f); // green
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f); // red
    public float feedbackDelay = 1f;                       // seconds to show color. Delay to each after quiz question


    private List<QuizQuestion> currentQuestions; // Rereference to current quiz questions
    private int currentIndex = 0; // which question we are on
    public int score = 0; // how many correct so far... 
    private Color[] originalButtonColors; //ignore
    public GameObject[] disablePlaneAfterQuiz, disableButtonAfterQuiz; //


    public int currentNpcId; // To track which NPC is being helped
    public NpcAnimation[] npcAnimation; // Reference to NpcAnimation script


    public GameNotifier gameNotifier; // Reference to GameNotifier script
    public AudioManager aud;

    private

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
            EndQuiz(currentNpcId);
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

                Debug.Log($"Setting up button {i}: active={btn.gameObject.activeSelf}, choiceIndex={i}, choices.Length={q.choices.Length}");
            }
            else
            {
                // hide unused buttons (if any)
                btn.gameObject.SetActive(false);
            }
        }



        

        Debug.Log($"Question: {q.question}, Choices={q.choices.Length}, Buttons={choiceButtons.Length}, CorrectIndex={q.correctIndex}");

    }
















    // central handler for button clicks
    public void OnChoiceButtonClicked(int choiceIndex)
    {
            Debug.Log($"OnChoiceButtonClicked called with index {choiceIndex}, buttons={choiceButtons.Length}, choices={currentQuestions[currentIndex].choices.Length}");
    if (currentQuestions == null || currentIndex >= currentQuestions.Count) return;
    if (choiceIndex < 0 || choiceIndex >= choiceButtons.Length) {
        Debug.LogError($"Choice index {choiceIndex} is out of bounds for choiceButtons.Length={choiceButtons.Length}");
        return;
    }
    QuizQuestion q = currentQuestions[currentIndex];
    if (choiceIndex >= q.choices.Length) {
        Debug.LogError($"Choice index {choiceIndex} is out of bounds for choices.Length={q.choices.Length}");
        return;
    }
    bool correct = (choiceIndex == q.correctIndex);


        // if (currentQuestions == null || currentIndex >= currentQuestions.Count) return; 
            


        // QuizQuestion q = currentQuestions[currentIndex]; 
        // bool correct = (choiceIndex == q.correctIndex);
        

        if (correct)
        {
            score++;
            DataManager.Instance.individualNpcScores[currentNpcId - 1]++; // ✅ track only one point per correct answer
            DataManager.Instance.playerScore_erudition++; // ✅ track only one point per correct answer
        }

        // disable buttons
        for (int i = 0; i < choiceButtons.Length; i++)
            choiceButtons[i].interactable = false;


        // color feedback
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
        // Wait scaled time. If you pause the game with Time.timeScale = 0 during quizzes,
        // change this to WaitForSecondsRealtime(delay).
        yield return new WaitForSeconds(delay);

        currentIndex++;
        ShowQuestion(); 
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = $"Score: {score}/{currentQuestions?.Count ?? 0}";
      //  if (Scoree != null) Scoree.text = $"Score: {score}/{currentQuestions?.Count ?? 0}";
    }

    void UpdateDataManager()
    {
        // DataManager.Instance.playerScore += score; //Adds current score to the total score

        DataManager.Instance.totalQuestionsAnswered++; // current question index (1-based)
    }

    void EndQuiz(int idToDisableTriggers)
    {
        Debug.Log("Quiz finished. Final score: " + score);
        // show result UI here or fire event
        gameObject.SetActive(false);
        disableButtonAfterQuiz[idToDisableTriggers - 1].SetActive(false);
        disablePlaneAfterQuiz[idToDisableTriggers - 1].SetActive(false);


        //Play NPC animation and dissapear
        npcAnimation[currentNpcId - 1].PlayAndDisappear(currentNpcId);


        //Game notif about earned points
        if(score > 0) gameNotifier.EarnedPoints(score); aud.PlaySFX(9); // play added points sfx
    }
}
