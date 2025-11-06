using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireSafetyQuiz : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizPanel;
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;

    [Header("Player Reference")]
    public Movements2 playerMovement;

    [Header("UI to Hide")]
    public SubtitleManager2 subtitleManager;
    public GameObject healthBar;
    public GameObject oxygenBar;

    private bool answeredCorrectly = false;
    private int correctAnswerIndex;
    private System.Action onQuizComplete;
    public GameNotifier gameNotifier;

    void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
            Debug.Log("Added listener to button " + i);
        }
    }

    public void ShowQuiz(string question, string[] answers, int correctIndex, System.Action onComplete = null)
    {
        Debug.Log("ShowQuiz called with question: " + question);
        Debug.Log("Number of answers: " + answers.Length);

        // Reset for new question
        answeredCorrectly = false;
        onQuizComplete = onComplete;
        correctAnswerIndex = correctIndex;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            Debug.Log("Player movement disabled");
        }

        if (subtitleManager != null)
            subtitleManager.HideObjective();
        if (healthBar != null)
            healthBar.SetActive(false);
        if (oxygenBar != null)
            oxygenBar.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor unlocked and visible");

        if (quizPanel != null)
        {
            quizPanel.SetActive(true);
            Debug.Log("Quiz panel activated");
        }

        if (questionText != null)
        {
            questionText.text = question;
            Debug.Log("Question text set to: " + questionText.text);
        }
        else
        {
            Debug.LogError("questionText is NULL!");
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < answers.Length)
            {
                answerButtons[i].gameObject.SetActive(true);
                answerTexts[i].text = answers[i];
                Debug.Log("Answer " + i + " set to: " + answers[i]);

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

        if (selectedIndex == correctAnswerIndex)
        {
            Debug.Log("Correct answer selected!");
            answeredCorrectly = true;

            Image btnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.green;

            StartCoroutine(CloseQuizAfterDelay(2f));
        }
        else
        {
            Debug.Log("Wrong answer selected!");
            answeredCorrectly = false;

            // Show wrong answer in red
            Image wrongBtnImage = answerButtons[selectedIndex].GetComponent<Image>();
            if (wrongBtnImage != null)
                wrongBtnImage.color = Color.red;

            // Show correct answer in green
            Image correctBtnImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            if (correctBtnImage != null)
                correctBtnImage.color = Color.green;

            StartCoroutine(CloseQuizAfterDelay(3f));
        }

        Debug.Log("All buttons disabled");
    }

    IEnumerator CloseQuizAfterDelay(float delay)
    {
        Debug.Log("Starting close quiz delay: " + delay + " seconds");
        yield return new WaitForSeconds(delay);

        // Award 1 point ONLY if answered correctly
        if (answeredCorrectly)
        {
            DataManager.Instance.quizScore += 1;
            DataManager.Instance.totalQuestionsAnswered += 1;

            AudioManager.Instance.PlaySFX(8);

            if (gameNotifier != null)
            {
                gameNotifier.EarnedPoints(1, 3f); // Always shows "+1 Point Earned!"
            }

            Debug.Log($"Awarded 1 point. Total quiz score: {DataManager.Instance.quizScore}");
        }
        else
        {
            // Wrong answer - 0 points awarded
            Debug.Log("Wrong answer - 0 points awarded");
        }

        Debug.Log("Closing quiz now");

        // Reset button colors and interactivity
        foreach (Button btn in answerButtons)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = Color.white;
            btn.interactable = true;
        }

        // Execute callback first
        Debug.Log("Executing quiz complete callback");
        onQuizComplete?.Invoke();

        // Hide quiz panel
        if (quizPanel != null)
            quizPanel.SetActive(false);

        // Restore player control and UI
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("Player movement re-enabled");
        }

        if (healthBar != null)
            healthBar.SetActive(true);
        if (oxygenBar != null)
            oxygenBar.SetActive(true);

        // Reset for next quiz
        answeredCorrectly = false;
    }
}