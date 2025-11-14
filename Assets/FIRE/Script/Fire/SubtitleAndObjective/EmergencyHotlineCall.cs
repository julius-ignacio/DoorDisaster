using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class EmergencyHotlineCall : MonoBehaviour
{
    public static bool IsHotlineActive = false;

    // ✅ Make this static so it persists across scene loads
    private static bool _hasCalledHotline = false;

    [Header("UI References")]
    public GameObject phoneUI;

    [Header("Answer Buttons (Must be 4 buttons)")]
    public Button[] numberButtons = new Button[4];

    [Header("Other UI Elements")]
    public GameObject dialingUI;
    public TextMeshProUGUI questionText;
    public Button skipButton;

    [Header("Subtitle Panel (for dispatcher dialogue)")]
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;

    [Header("Dependencies")]
    public SubtitleManager2 subtitleManager;
    public GameObject healthBar;
    public GameObject oxygenBar;

    [Header("Hotline Completion Tracker")]
    public GameObject hotlineCompletionFlag; // ✅ NEW: Drag the HotlineCompletionFlag object here

    [Header("Audio")]
    public int dialToneSFX = 34;
    public int dispatcherVoiceSFX = 35;
    public int wrongNumberSFX = 36;

    private QuizQuestion2 currentQuiz;
    private Coroutine typingCoroutine;
    private bool skipDialogue = false;

    void Start()
    {
        if (phoneUI != null) phoneUI.SetActive(false);
        if (dialingUI != null) dialingUI.SetActive(false);

        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false);
            skipButton.onClick.AddListener(OnSkipButtonPressed);
        }
    }

    public void TriggerHotlineObjective()
    {
        if (_hasCalledHotline) return;
        ShowEmergencyNumberQuiz();
    }

    void ShowEmergencyNumberQuiz()
    {
        IsHotlineActive = true;

        currentQuiz = QuizDatabase2.GetQuiz("emergency_number");

        if (currentQuiz != null)
        {
            if (phoneUI != null)
            {
                phoneUI.SetActive(true);
                Time.timeScale = 0f;

                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.Joystick != null)
                        GameManager.Instance.Joystick.SetActive(false);
                    if (GameManager.Instance.Jumpbtn != null)
                        GameManager.Instance.Jumpbtn.SetActive(false);
                }
            }

            if (subtitleManager != null)
                subtitleManager.HideObjective();

            if (healthBar != null)
                healthBar.SetActive(false);

            if (oxygenBar != null)
                oxygenBar.SetActive(false);

            if (questionText != null)
                questionText.text = currentQuiz.question;

            for (int i = 0; i < numberButtons.Length && i < currentQuiz.answers.Length; i++)
            {
                int answerIndex = i;

                TextMeshProUGUI btnText = numberButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = currentQuiz.answers[i];

                numberButtons[i].onClick.RemoveAllListeners();
                numberButtons[i].onClick.AddListener(() => OnAnswerSelected(answerIndex));
                numberButtons[i].gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Quiz 'emergency_number' not found in database!");
        }
    }

    void OnAnswerSelected(int selectedIndex)
    {
        Debug.Log($"Answer selected: {selectedIndex}, Correct: {currentQuiz.correctAnswerIndex}");

        if (selectedIndex == currentQuiz.correctAnswerIndex)
        {
            StartCoroutine(MakeEmergencyCall());
        }
        else
        {
            StartCoroutine(HandleWrongNumber(selectedIndex));
        }
    }

    IEnumerator HandleWrongNumber(int wrongIndex)
    {
        Debug.Log("Wrong number selected: " + wrongIndex);

        foreach (Button btn in numberButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (dialingUI != null)
            dialingUI.SetActive(true);

        if (questionText != null)
            questionText.text = "Dialing " + currentQuiz.answers[wrongIndex] + "...";

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(dialToneSFX);

        yield return new WaitForSecondsRealtime(2f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAll();

        if (dialingUI != null)
            dialingUI.SetActive(false);

        string[] wrongNumberMessages = new string[]
        {
            "You've reached Red Cross emergency services. For fire emergencies, please call 911 immediately.",
            "This is the emergency hotline. For fire emergencies, please dial 911.",
            "This is the citizens' complaint and information hotline. For emergencies, please dial 911.",
            "You've reached the wrong number. For fire emergencies, please call 911."
        };

        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        if (subtitleText != null)
        {
            string message = wrongIndex < wrongNumberMessages.Length
                ? wrongNumberMessages[wrongIndex]
                : "This is not the emergency number. Please call 911.";

            subtitleText.text = message;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(wrongNumberSFX);

        yield return new WaitForSecondsRealtime(4f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAll();

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);

        if (questionText != null)
            questionText.text = "That was the wrong number! Try again!";

        yield return new WaitForSecondsRealtime(2f);

        ShowEmergencyNumberQuiz();
    }

    IEnumerator MakeEmergencyCall()
    {
        Debug.Log("Calling 911 - correct answer!");
        _hasCalledHotline = true;

        foreach (Button btn in numberButtons)
        {
            btn.gameObject.SetActive(false);
        }

        if (dialingUI != null)
            dialingUI.SetActive(true);

        if (questionText != null)
            questionText.text = "Calling 911...";

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(dialToneSFX);

        yield return new WaitForSecondsRealtime(2f);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAll();

        if (dialingUI != null)
            dialingUI.SetActive(false);

        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        if (questionText != null)
            questionText.text = "911 Emergency - Call Connected";

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(dispatcherVoiceSFX);

        yield return ShowDispatcherDialogue();

        EndCall();
    }

    IEnumerator ShowDispatcherDialogue()
    {
        if (skipButton != null)
            skipButton.gameObject.SetActive(true);

        string[] dispatcherLines = new string[]
        {
            "911 Emergency, what's your emergency?",
            "There's a fire in my house! I need help!",
            "Stay calm. Help is on the way. Can you get out safely?",
            "Yes, but I need to grab my essentials first - Phone, important documents!",
            "Alright, but be QUICK. Only grab what you absolutely need. Don't waste time.",
            "What about my cat? I think he's trapped somewhere!",
            "If it's safe to reach him, do it quickly. But if there's too much fire, let the firefighters handle it.",
            "Okay, I'll be fast. Thank you!",
            "Cover your mouth and get out as soon as possible. Firefighters are on their way."
        };

        for (int i = 0; i < dispatcherLines.Length; i++)
        {
            if (skipDialogue)
            {
                Debug.Log("Dialogue skipped!");

                if (AudioManager.Instance != null)
                    AudioManager.Instance.StopAll();

                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                break;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopAll();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(dispatcherVoiceSFX);

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(TypeSubtitle(dispatcherLines[i], 2f));

            yield return new WaitForSecondsRealtime(2f);

            if (skipDialogue)
            {
                Debug.Log("Dialogue skipped!");

                if (AudioManager.Instance != null)
                    AudioManager.Instance.StopAll();

                break;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopAll();

            yield return new WaitForSecondsRealtime(3f);
        }

        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        skipDialogue = false;
    }

    IEnumerator TypeSubtitle(string text, float duration)
    {
        if (subtitleText == null) yield break;

        subtitleText.text = "";
        float delay = duration / Mathf.Max(1, text.Length);

        for (int i = 0; i < text.Length; i++)
        {
            subtitleText.text += text[i];
            yield return new WaitForSecondsRealtime(delay);
        }
    }

    void EndCall()
    {
        Debug.Log("✅ Call ended - hotline completed!");

        IsHotlineActive = false;

        // ✅ NEW: Deactivate the tracker flag (gets saved by WorldSaveSystem)
        if (hotlineCompletionFlag != null)
        {
            hotlineCompletionFlag.SetActive(false);
            Debug.Log("✅ Hotline completion flag deactivated (will be saved)");
        }

        if (phoneUI != null)
            phoneUI.SetActive(false);

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.Joystick != null)
                GameManager.Instance.Joystick.SetActive(true);
            if (GameManager.Instance.Jumpbtn != null)
                GameManager.Instance.Jumpbtn.SetActive(true);
        }

        if (healthBar != null)
            healthBar.SetActive(true);

        if (oxygenBar != null)
            oxygenBar.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAll();

        if (subtitleManager != null)
        {
            SubtitleManager2.CallObjectiveActive = false;
            HandCoverPickup.DoorObjectiveActive = true;

            subtitleManager.ShowCustomMessage(
                "Help is on the way! Let me grab my backpack first.",
                3f,
                () =>
                {
                    subtitleManager.ShowObjective("Pick up your backpack");
                }
            );
        }

        Debug.Log("✅ _hasCalledHotline = " + _hasCalledHotline);
    }

    public bool HasCalledHotline()
    {
        return _hasCalledHotline;
    }

    public static void ResetForNewTrial()
    {
        _hasCalledHotline = false;
        Debug.Log("EmergencyHotlineCall flag reset for new trial");
    }

    void OnSkipButtonPressed()
    {
        skipDialogue = true;
        Debug.Log("Skip button pressed!");
    }
}