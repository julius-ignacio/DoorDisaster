using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireSafetyQuiz : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
<<<<<<< HEAD
    public Button[] answerButtons; // Array of answer buttons (3-4 buttons)
    public TextMeshProUGUI[] answerTexts; // Text on each button

    [Header("Player Reference")]
    public Movements playerMovement; // To disable movement during quiz

    [Header("UI to Hide")]
    public SubtitleManager subtitleManager; // To hide objectives
=======
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;

    [Header("Player Reference")]
    public Movements playerMovement;

    [Header("UI to Hide")]
    public SubtitleManager subtitleManager;
>>>>>>> 47c3962 (Quiz script changes)
    public GameObject healthBar;
    public GameObject oxygenBar;

    private int correctAnswerIndex;
    private System.Action onQuizComplete;

    void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

<<<<<<< HEAD
        // Setup button listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture for closure
=======
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
>>>>>>> 47c3962 (Quiz script changes)
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            Debug.Log("Added listener to button " + i);
        }
    }

<<<<<<< HEAD
=======
    private bool isLastQuestion = false;

>>>>>>> 47c3962 (Quiz script changes)
    public void ShowQuiz(string question, string[] answers, int correctIndex, System.Action onComplete = null)
    {
        Debug.Log("ShowQuiz called with question: " + question);
        Debug.Log("Number of answers: " + answers.Length);

<<<<<<< HEAD
        // Store callback
        onQuizComplete = onComplete;
        correctAnswerIndex = correctIndex;

        // Disable player movement
=======
        onQuizComplete = onComplete;
        correctAnswerIndex = correctIndex;

>>>>>>> 47c3962 (Quiz script changes)
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("Player movement disabled");
        }

<<<<<<< HEAD
        // Hide UI elements
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (subtitleManager != null)
            subtitleManager.HideObjective();
        if (healthBar != null)
            healthBar.SetActive(false);
        if (oxygenBar != null)
            oxygenBar.SetActive(false);

<<<<<<< HEAD
        // Show cursor for quiz
=======
>>>>>>> 47c3962 (Quiz script changes)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor unlocked and visible");

<<<<<<< HEAD
        // Show quiz panel
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Debug.Log("Quiz panel activated");
        }

<<<<<<< HEAD
        // Set question
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (questionText != null)
        {
            questionText.text = question;
            Debug.Log("Question text set to: " + questionText.text);
        }
        else
        {
            Debug.LogError("questionText is NULL!");
        }

<<<<<<< HEAD
        // Set answers
=======
>>>>>>> 47c3962 (Quiz script changes)
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = answers[i];
                Debug.Log("Answer " + i + " set to: " + answers[i]);

<<<<<<< HEAD
                // Reset button colors
=======
>>>>>>> 47c3962 (Quiz script changes)
                ColorBlock colors = answerButtons[i].colors;
                colors.normalColor = Color.white;
                answerButtons[i].colors = colors;
                answerButtons[i].interactable = true;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }
    }

<<<<<<< HEAD
=======
    // Call this to mark that the next quiz is the last one
    public void SetLastQuestion(bool isLast)
    {
        isLastQuestion = isLast;
    }

>>>>>>> 47c3962 (Quiz script changes)
    void OnAnswerSelected(int selectedIndex)
    {
        Debug.Log("OnAnswerSelected called with index: " + selectedIndex);
        Debug.Log("Correct answer index is: " + correctAnswerIndex);

<<<<<<< HEAD
        // Disable all buttons to prevent multiple clicks
        foreach (Button btn in answerButtons)
            btn.interactable = false;

        // Check if correct
=======
        foreach (Button btn in answerButtons)
            btn.interactable = false;

>>>>>>> 47c3962 (Quiz script changes)
        if (selectedIndex == correctAnswerIndex)
        {
            Debug.Log("Correct answer selected!");

<<<<<<< HEAD
            // Change button image color directly
=======
            // Add 1 point for correct answer
            DataManager.instance.AddQuizScore(1);

>>>>>>> 47c3962 (Quiz script changes)
            Image btnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.green;

<<<<<<< HEAD
            // Close quiz after 2 seconds so player can read
=======
>>>>>>> 47c3962 (Quiz script changes)
            StartCoroutine(CloseQuizAfterDelay(2f));
        }
        else
        {
            Debug.Log("Wrong answer selected!");

<<<<<<< HEAD
            // Change wrong button to red
=======
>>>>>>> 47c3962 (Quiz script changes)
            Image wrongBtnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (wrongBtnImage != null)
                wrongBtnImage.color = Color.red;

<<<<<<< HEAD
            // Change correct button to green
=======
>>>>>>> 47c3962 (Quiz script changes)
            Image correctBtnImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            if (correctBtnImage != null)
                correctBtnImage.color = Color.green;

<<<<<<< HEAD
            // Close quiz after 3 seconds so player can see both colors
=======
>>>>>>> 47c3962 (Quiz script changes)
            StartCoroutine(CloseQuizAfterDelay(3f));
        }

        Debug.Log("All buttons disabled");
    }

    IEnumerator CloseQuizAfterDelay(float delay)
    {
        Debug.Log("Starting close quiz delay: " + delay + " seconds");
        yield return new WaitForSeconds(delay);

        Debug.Log("Closing quiz now");

<<<<<<< HEAD
        // Hide quiz
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Reset button colors back to white
=======
>>>>>>> 47c3962 (Quiz script changes)
        foreach (Button btn in answerButtons)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.white;
            btn.interactable = true;
        }

<<<<<<< HEAD
        // Re-enable player movement
=======
        // Execute callback first
        Debug.Log("Executing quiz complete callback");
        onQuizComplete?.Invoke();

        // Hide quiz panel ALWAYS
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Always restore player control and UI after any quiz
>>>>>>> 47c3962 (Quiz script changes)
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("Player movement re-enabled");
        }

<<<<<<< HEAD
        // Lock cursor again for gameplay
=======
>>>>>>> 47c3962 (Quiz script changes)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked again");

<<<<<<< HEAD
        // Show UI elements again
=======
>>>>>>> 47c3962 (Quiz script changes)
        if (healthBar != null)
            healthBar.SetActive(true);
        if (oxygenBar != null)
            oxygenBar.SetActive(true);
<<<<<<< HEAD

        // Execute callback (this will show the objective)
        Debug.Log("Executing quiz complete callback");
        onQuizComplete?.Invoke();
=======
>>>>>>> 47c3962 (Quiz script changes)
    }
}