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

    [Header("Inventory UI")]
    public GameObject backpackButton; // ✅ Added reference for backpack button

    [Header("Story Settings")]
    public bool autoStartStory = true;

    // ✅ Flags
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

    // ✅ NEW: Track if objective was visible before pause
    private bool objectiveWasVisibleBeforePause = false;

    void Start()
    {
        IntroStoryComplete = false;
        CallObjectiveActive = false;

        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (objectivePanel != null) objectivePanel.SetActive(false);
        if (healthBar != null) healthBar.SetActive(false);
        if (oxygenBar != null) oxygenBar.SetActive(false);
        if (backpackButton != null) backpackButton.SetActive(false); // ✅ Hide backpack at start

        if (autoStartStory)
            StartCoroutine(PlayWakeUpStory());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            SkipCurrentText();
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
        Debug.Log("Intro story complete - pickups now enabled");

        Cursor.lockState = CursorLockMode.None;

        Movements2 player = FindObjectOfType<Movements2>();
        if (player != null)
            player.ForceEnable();

        if (healthBar != null)
            healthBar.SetActive(true);

        if (oxygenBar != null)
            oxygenBar.SetActive(true);

        if (backpackButton != null)
            backpackButton.SetActive(true); // ✅ Show backpack after story

        PlayerOxygen oxygenSystem = FindObjectOfType<PlayerOxygen>();
        if (oxygenSystem != null)
            oxygenSystem.ShowOxygenBar();

        // ✅ Show "Find phone" objective
        CallObjectiveActive = true;
        ShowObjective("Find the phone and call for help!");
    }

    IEnumerator ShowSubtitle(string text, float displayTime, Action onComplete = null)
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        IsSubtitleActive = true;
        subtitleJustFinished = false;

        if (subtitleText != null)
        {
            subtitleText.text = "";

            if (currentTyping != null)
                StopCoroutine(currentTyping);

            currentTyping = StartCoroutine(TypeText(text));
            yield return currentTyping;

            yield return new WaitForSeconds(displayTime);

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
            subtitleText.text = text.Substring(0, i);
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void SkipCurrentText()
    {
        if (currentTyping != null)
        {
            StopCoroutine(currentTyping);
            currentTyping = null;
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
        if (objectivePanel != null)
            objectivePanel.SetActive(false);
    }

    public void RestoreLastObjective()
    {
        if (!string.IsNullOrEmpty(lastObjective))
            ShowObjective(lastObjective);
    }

    public void OnPause()
    {
        // ✅ Track if objective was actually visible before hiding
        objectiveWasVisibleBeforePause = (objectivePanel != null && objectivePanel.activeSelf);

        if (subtitlePanel != null && subtitlePanel.activeSelf)
            subtitlePanel.SetActive(false);
        if (objectivePanel != null && objectivePanel.activeSelf)
            objectivePanel.SetActive(false);
    }

    public void OnResume()
    {
        // Resume subtitle only if it was actively typing
        if (IsSubtitleActive && subtitleText != null && !string.IsNullOrEmpty(subtitleText.text) && !subtitleJustFinished)
            subtitlePanel.SetActive(true);

        // ✅ FIXED: Only restore objective if it was visible before pause
        if (objectiveWasVisibleBeforePause && !string.IsNullOrEmpty(lastObjective))
        {
            objectivePanel.SetActive(true);
        }

        // Reset the tracking flag
        objectiveWasVisibleBeforePause = false;
    }

    public void HideAll()
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
        if (objectivePanel != null)
            objectivePanel.SetActive(false);
        if (subtitleText != null)
            subtitleText.text = "";
        if (objectiveText != null)
            objectiveText.text = "";
        IsSubtitleActive = false;
        subtitleJustFinished = true;
    }
}
