using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class SubtitleManager2 : MonoBehaviour
{
    [Header("Subtitle UI (Bottom)")]
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;
    public float typingSpeed = 0.02f;

    [Header("Objective UI")]
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveText;

    [Header("Health UI")]
    public GameObject healthBar;

    [Header("Oxygen UI")]
    public GameObject oxygenBar;

    [Header("Story Settings")]
    public bool autoStartStory = true;

    public static bool IntroStoryComplete { get; private set; } = false;
    public static bool IsSubtitleActive { get; private set; } = false;
    public static bool CallObjectiveActive { get; set; } = false;

    private string[] wakingStory = {
        "*sniff sniff*",
        "That smells like... smoke?",
        "Oh no... OH NO! Something's burning!",
        "The kitchen! I left the stove on! The house is filling with smoke!",
        "I need to get out of here NOW!"
    };

    private Coroutine currentTyping;
    private string lastObjective = "";
    private bool subtitleJustFinished = false;
    private bool objectiveWasVisibleBeforePause = false;
    private bool wasTypingBeforeInventoryOpen = false;
    private string currentSubtitleText = "";
    private float currentSubtitleDuration = 0f;

    void Start()
    {
        IntroStoryComplete = false;
        CallObjectiveActive = false;

        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (healthBar != null) healthBar.SetActive(false);
        if (oxygenBar != null) oxygenBar.SetActive(false);

        // NEW: If wake-up is already done/disabled, skip the story and bring up HUD/objective
        var wake = FindObjectOfType<WakeUpController>(true);
        if ((wake == null || !wake.enabled) && !IntroStoryComplete)
        {
            ForceIntroComplete();
            return; // Don't start the story
        }

        if (autoStartStory)
            StartCoroutine(PlayWakeUpStory());
    }

    public static void ForceIntroComplete()
    {
        if (IntroStoryComplete) return;
        IntroStoryComplete = true;

        var player = FindObjectOfType<Movements2>(true);
        if (player != null)
        {
            if (!player.gameObject.activeSelf) player.gameObject.SetActive(true);
            if (!player.enabled) player.enabled = true;
            var cc = player.GetComponent<CharacterController>();
            if (cc != null && !cc.enabled) cc.enabled = true;
        }

        var inst = FindObjectOfType<SubtitleManager2>(true);
        if (inst != null)
        {
            Cursor.lockState = CursorLockMode.None;

            if (inst.healthBar != null)
                inst.healthBar.SetActive(true);

            if (inst.oxygenBar != null)
                inst.oxygenBar.SetActive(true);

            var oxygenSystem = FindObjectOfType<PlayerOxygen>(true);
            if (oxygenSystem != null)
                oxygenSystem.ShowOxygenBar();

            CallObjectiveActive = true;
            inst.ShowObjective("Find the phone and call for help!");
        }
    }

    public IEnumerator PlayWakeUpStory()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < wakingStory.Length; i++)
        {
            float duration = (i == 0) ? 1.5f : 2.5f;
            yield return StartCoroutine(ShowSubtitle(wakingStory[i], duration));
            yield return new WaitForSeconds(0.2f);
        }

        IntroStoryComplete = true;
        Cursor.lockState = CursorLockMode.None;

        var player = FindObjectOfType<Movements2>(true);
        if (player != null)
        {
            if (!player.enabled) player.enabled = true;
            var cc = player.GetComponent<CharacterController>();
            if (cc != null && !cc.enabled) cc.enabled = true;
        }

        if (healthBar != null) healthBar.SetActive(true);
        if (oxygenBar != null) oxygenBar.SetActive(true);

        var oxygenSystem = FindObjectOfType<PlayerOxygen>(true);
        if (oxygenSystem != null) oxygenSystem.ShowOxygenBar();

        CallObjectiveActive = true;
        ShowObjective("Find the phone and call for help!");
    }

    IEnumerator ShowSubtitle(string text, float displayTime, Action onComplete = null)
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(true);

        IsSubtitleActive = true;
        subtitleJustFinished = false;
        currentSubtitleText = text;
        currentSubtitleDuration = displayTime;

        if (subtitleText != null)
        {
            subtitleText.text = "";
            if (currentTyping != null) StopCoroutine(currentTyping);
            currentTyping = StartCoroutine(TypeText(text));
            yield return currentTyping;

            if (subtitleText != null) subtitleText.text = text;

            float elapsedTime = 0f;
            while (elapsedTime < displayTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            subtitlePanel.SetActive(false);
            IsSubtitleActive = false;
            subtitleJustFinished = true;

            onComplete?.Invoke();
        }
    }

    IEnumerator TypeText(string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            if (subtitleText != null) subtitleText.text = text.Substring(0, i);
            float elapsed = 0f;
            while (elapsed < typingSpeed)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }

    public void ShowCustomMessage(string message, float duration, Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ShowSubtitle(message, duration, onComplete));
    }

    public void ShowObjective(string text)
    {
        if (objectivePanel != null && objectiveText != null)
        {
            lastObjective = text;
            objectiveText.text = "Objective: " + text;
            objectivePanel.SetActive(true);
        }
    }

    public void HideObjective()
    {
        if (objectivePanel != null) objectivePanel.SetActive(false);
    }

    public void RestoreLastObjective()
    {
        if (!string.IsNullOrEmpty(lastObjective)) ShowObjective(lastObjective);
    }

    public void OnPause()
    {
        objectiveWasVisibleBeforePause = (objectivePanel != null && objectivePanel.activeSelf);
        if (subtitlePanel != null && subtitlePanel.activeSelf) subtitlePanel.SetActive(false);
        if (objectivePanel != null && objectivePanel.activeSelf) objectivePanel.SetActive(false);
    }

    public void OnResume()
    {
        if (IsSubtitleActive && subtitleText != null && !string.IsNullOrEmpty(subtitleText.text) && !subtitleJustFinished)
            subtitlePanel.SetActive(true);

        if (objectiveWasVisibleBeforePause && !string.IsNullOrEmpty(lastObjective))
            objectivePanel.SetActive(true);

        objectiveWasVisibleBeforePause = false;
    }

    public void OnInventoryOpen()
    {
        wasTypingBeforeInventoryOpen = IsSubtitleActive;
        if (IsSubtitleActive && subtitlePanel != null)
            Debug.Log("Inventory opened during subtitle - subtitle continues");
    }

    public void OnInventoryClose()
    {
        Debug.Log("Inventory closed - subtitle resumes normally");
    }

    public void HideAll()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (subtitleText != null) subtitleText.text = "";
        if (objectiveText != null) objectiveText.text = "";
        IsSubtitleActive = false;
        subtitleJustFinished = true;
    }
}