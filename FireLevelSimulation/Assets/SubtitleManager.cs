using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    [Header("Subtitle UI (Bottom)")]
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;
    public float typingSpeed = 0.05f;

    [Header("Objective UI")]
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveText;

    [Header("Story Settings")]
    public bool autoStartStory = true;

    private string[] wakingStory = {
        "*sniff sniff*",
        "That smells like... smoke?",
        "Oh no... OH NO! Something's burning!",
        "The kitchen! I left the stove on! The house is filling with smoke!",
        "I need to get out of here NOW!"
    };

    private string[] doorCheckStory = {
        "I need to check if the door is hot before opening it",
        "The door feels warm but not burning hot",
        "It should be safe to open"
    };

    private string[] hallwayStory = {
        "The hallway is filling with smoke!",
        "I need to stay low and find an exit",
        "The front door... I can see flames blocking it!"
    };

    private string[] windowStory = {
        "The window! That's my way out!",
        "I need to break it and get outside",
        "Almost there... just a little more!"
    };

    private Coroutine currentTyping;

    void Start()
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);

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
            float duration = (i == 0) ? 2f : 3f;
            yield return StartCoroutine(ShowSubtitle(wakingStory[i], duration));
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator ShowSubtitle(string text, float displayTime, Action onComplete = null)
    {
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);

        if (subtitleText != null)
        {
            subtitleText.text = "";

            if (currentTyping != null)
                StopCoroutine(currentTyping);

            currentTyping = StartCoroutine(TypeText(text));
            yield return currentTyping;

            yield return new WaitForSeconds(displayTime);
            subtitlePanel.SetActive(false);

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
            if (subtitleText != null)
                subtitleText.text = subtitleText.text;
        }
    }

    public void ShowCustomMessage(string message, float duration, Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ShowSubtitle(message, duration, onComplete));
    }

    // --- Updated Methods ---
    public void ShowObjective(string text)
    {
        if (objectivePanel != null && objectiveText != null)
        {
            objectiveText.text = "Objective: " + text;
            objectivePanel.SetActive(true);
        }
    }

    public void HideObjective()
    {
        if (objectivePanel != null && objectiveText != null)
        {
            objectivePanel.SetActive(false);
            objectiveText.text = ""; // Clear text when hiding
        }
    }
}
