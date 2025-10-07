using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MilkShake;

public class QuizScript : MonoBehaviour
{

    public GameObject helpBtn; //opens quiz panel
    public GameObject quizUI; //quiz panel.. drag and drop in inspector the quiz panel OBJECT hereee
    public GameObject hud; //quiz panel.. drag and drop in inspector the quiz panel OBJECT hereee


    [Header("UI")]
    public TMP_Text questionText;
    public Button[] choiceButtons;            // assign in inspector (e.g. 3 buttons)
    public TMP_Text scoreText;               // shows player score
                                             // public TMP_Text Scoree;                  // result text you used wala lang to pang test langggg ignore this

    [Header("Colors & Timing")] //Ignore these
    public Color correctColor = new Color(0.2f, 0.8f, 0.2f); // green
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f); // red
    public float feedbackDelay = 1f;                       // seconds to show color. Delay to each after quiz question


    [Header("Disable movements and quake whent taking quiz")]
    public Movements movements;
    public ConsistentQuake consistentQuake;
    public Shaker shake;



    private List<QuizQuestion> currentQuestions; // Rereference to current quiz questions
    private int currentIndex = 0; // which question we are on
    public int score = 0; // how many correct so far... 
    private Color[] originalButtonColors; //ignore
    public GameObject[] disablePlaneAfterQuiz; //


    public int currentNpcId; // To track which NPC is being helped
    public NpcAnimation[] npcAnimation; // Reference to NpcAnimation script



    void Start()
    {
        quizUI.SetActive(false);
        helpBtn.SetActive(false);
    }


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



    public void OnHelpButtonClick()
    {
        helpBtn.SetActive(false);
        quizUI.SetActive(true);

        List<QuizQuestion> questions = null;
        switch (currentNpcId)
        {
            case 1: questions = QuizDatabase.NPC1; break;
            case 2: questions = QuizDatabase.NPC2; break;
            case 3: questions = QuizDatabase.NPC3; break;
            case 4: questions = QuizDatabase.NPC4; break;
            case 5: questions = QuizDatabase.NPC5; break;
            case 6: questions = QuizDatabase.Medkit; break;
            case 7: questions = QuizDatabase.Medkit2; break;
            case 8: questions = QuizDatabase.Medkit3; break;
            case 9: questions = QuizDatabase.Water1; break;
            case 10: questions = QuizDatabase.Water2; break;
            case 11: questions = QuizDatabase.Water3; break;
            case 12: questions = QuizDatabase.Whistle1; break;

        }

        BeginQuiz(questions);



    }












    //Is called when the player clicks the BUTTON in the other script 
    public void BeginQuiz(List<QuizQuestion> questions)
    {
        movements.enabled = false;

        shake.enabled = false;
        consistentQuake.enabled = false;

        hud.SetActive(false);


        consistentQuake.PauseQuakes();


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

                Debug.Log($"Button {i} interactable={btn.interactable}, hasListener={btn.onClick.GetPersistentEventCount()}");

                // remove previous listeners then add new one
                btn.onClick.RemoveAllListeners();

                Debug.Log($"Button {i} interactable={btn.interactable}, hasListener={btn.onClick.GetPersistentEventCount()}");

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
        Debug.Log($"Answer clicked for NPC {currentNpcId}, writing to index {currentNpcId - 1}");

        Debug.Log($"OnChoiceButtonClicked called with index {choiceIndex}, buttons={choiceButtons.Length}, choices={currentQuestions[currentIndex].choices.Length}");
        if (currentQuestions == null || currentIndex >= currentQuestions.Count) return;
        if (choiceIndex < 0 || choiceIndex >= choiceButtons.Length)
        {
            Debug.LogError($"Choice index {choiceIndex} is out of bounds for choiceButtons.Length={choiceButtons.Length}");
            return;
        }
        QuizQuestion q = currentQuestions[currentIndex];
        if (choiceIndex >= q.choices.Length)
        {
            Debug.LogError($"Choice index {choiceIndex} is out of bounds for choices.Length={q.choices.Length}");
            return;
        }
        bool correct = choiceIndex == q.correctIndex;


        // if (currentQuestions == null || currentIndex >= currentQuestions.Count) return; 



        // QuizQuestion q = currentQuestions[currentIndex]; 
        // bool correct = (choiceIndex == q.correctIndex);


        if (correct)
        {
            score++;
            Debug.Log($"Correct answer for NPC {currentNpcId}");

            // ✅ Only track GLOBAL score here
            DataManager.Instance.quizScore++;


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

        Debug.Log($"OnChoiceButtonClicked fired! choiceIndex={choiceIndex}, correct={correct}");




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
        Debug.Log($"Quiz finished. NPC {currentNpcId}, Score={score}");

        // ✅ Save this NPC's score
        DataManager.Instance.npcScores[currentNpcId] = score;

        disablePlaneAfterQuiz[idToDisableTriggers - 1].SetActive(false);
        npcAnimation[currentNpcId - 1].PlayAndDisappear(currentNpcId);

        gameObject.SetActive(false);
        helpBtn.SetActive(false);


       ///////////////////// npcAnimation[currentNpcId - 1].ReactToScore(score);

        // if (score > 0)
        // {
        //     if (currentNpcId > 0 && currentNpcId <= 5)
        //     {
        //         gameNotifier.EarnedPoints(score);
        //         AudioManager.Instance.PlaySFX(8); //points

        //         if (npcsaved != null) { npcsaved.makeIconActive(); }
        //     }

        //     else if (currentNpcId >= 6 && currentNpcId <= 8)
        //     {
        //         AudioManager.Instance.PlaySFX(19); //medkit
        //         AudioManager.Instance.PlaySFX(8); //points
        //         gameNotifier.EarnedPoints(score);


        //     }

        //     else if (currentNpcId >= 9 && currentNpcId <= 11)
        //     {
        //         AudioManager.Instance.PlaySFX(18); //drink water
        //         AudioManager.Instance.PlaySFX(8); //points
        //         gameNotifier.EarnedPoints(score);


        //     }


        //     else if (currentNpcId >= 12)
        //     {
        //         gameNotifier.ObtainedItem(2, "Whistle");
        //         AudioManager.Instance.PlaySFX(8); //points
        //     }

        // }


        consistentQuake.enabled = true;
consistentQuake.ResumeQuakes();




        hud.SetActive(true);

        shake.enabled = true;
        movements.enabled = true;

    }


}
