using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireSafetyQuiz : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons; // Array of answer buttons (3-4 buttons)
    public TextMeshProUGUI[] answerTexts; // Text on each button

    [Header("Player Reference")]
    public Movements playerMovement; // To disable movement during quiz

    [Header("UI to Hide")]
    public SubtitleManager subtitleManager; // To hide objectives
    public GameObject healthBar;
    public GameObject oxygenBar;

    private int correctAnswerIndex;
    private System.Action onQuizComplete;

    void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Setup button listeners
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture for closure
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            Debug.Log("Added listener to button " + i);
        }
    }

    public void ShowQuiz(string question, string[] answers, int correctIndex, System.Action onComplete = null)
    {
        Debug.Log("ShowQuiz called with question: " + question);
        Debug.Log("Number of answers: " + answers.Length);

        // Store callback
        onQuizComplete = onComplete;
        correctAnswerIndex = correctIndex;

        // Disable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("Player movement disabled");
        }

        // Hide UI elements
        if (subtitleManager != null)
            subtitleManager.HideObjective();
        if (healthBar != null)
            healthBar.SetActive(false);
        if (oxygenBar != null)
            oxygenBar.SetActive(false);

        // Show cursor for quiz
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor unlocked and visible");

        // Show quiz panel
        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Debug.Log("Quiz panel activated");
        }

        // Set question
        if (questionText != null)
        {
            questionText.text = question;
            Debug.Log("Question text set to: " + questionText.text);
        }
        else
        {
            Debug.LogError("questionText is NULL!");
        }

        // Set answers
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = answers[i];
                Debug.Log("Answer " + i + " set to: " + answers[i]);

                // Reset button colors
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

    void OnAnswerSelected(int selectedIndex)
    {
        Debug.Log("OnAnswerSelected called with index: " + selectedIndex);
        Debug.Log("Correct answer index is: " + correctAnswerIndex);

        // Disable all buttons to prevent multiple clicks
        foreach (Button btn in answerButtons)
            btn.interactable = false;

        // Check if correct
        if (selectedIndex == correctAnswerIndex)
        {
            Debug.Log("Correct answer selected!");

            // Change button image color directly
            Image btnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.green;

            // Close quiz after 2 seconds so player can read
            StartCoroutine(CloseQuizAfterDelay(2f));
        }
        else
        {
            Debug.Log("Wrong answer selected!");

            // Change wrong button to red
            Image wrongBtnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (wrongBtnImage != null)
                wrongBtnImage.color = Color.red;

            // Change correct button to green
            Image correctBtnImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            if (correctBtnImage != null)
                correctBtnImage.color = Color.green;

            // Close quiz after 3 seconds so player can see both colors
            StartCoroutine(CloseQuizAfterDelay(3f));
        }

        Debug.Log("All buttons disabled");
    }

    IEnumerator CloseQuizAfterDelay(float delay)
    {
        Debug.Log("Starting close quiz delay: " + delay + " seconds");
        yield return new WaitForSeconds(delay);

        Debug.Log("Closing quiz now");

        // Hide quiz
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Reset button colors back to white
        foreach (Button btn in answerButtons)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.white;
            btn.interactable = true;
        }

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("Player movement re-enabled");
        }

        // Lock cursor again for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked again");

        // Show UI elements again
        if (healthBar != null)
            healthBar.SetActive(true);
        if (oxygenBar != null)
            oxygenBar.SetActive(true);

        // Execute callback (this will show the objective)
        Debug.Log("Executing quiz complete callback");
        onQuizComplete?.Invoke();
    }
}