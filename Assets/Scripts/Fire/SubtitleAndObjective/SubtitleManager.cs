using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubtitleManager2 : MonoBehaviour
{
    [Header("Subtitle UI (Bottom)")]
    public GameObject subtitlePanel;
    public TextMeshProUGUI subtitleText;
    public float typingSpeed = 0.05f;

    [Header("Objective UI")]
    public GameObject objectivePanel;
    public TextMeshProUGUI objectiveText;

    [Header("Health UI")]
    public GameObject healthBar; // Reference to your HP bar GameObject

    [Header("Oxygen UI")]
    public GameObject oxygenBar; // Reference to your Oxygen bar GameObject

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

        if (objectivePanel != null)
            objectivePanel.SetActive(false);

        if (healthBar != null)
            healthBar.SetActive(false);

        if (oxygenBar != null)
            oxygenBar.SetActive(false);

        // Disable movement during story
        Cursor.lockState = CursorLockMode.None;

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

        // ✅ FIXED: Enable movement after story
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ✅ Force enable player movement
        Movements2 player = FindObjectOfType<Movements2>();
        if (player != null)
        {
            player.ForceEnable();
            Debug.Log("Player movement force enabled after subtitles");
        }

        // ✅ Show HP bar after the story ends
        if (healthBar != null)
        {
            healthBar.SetActive(true);
            Debug.Log("Health bar shown");
        }

        // ✅ Show oxygen bar after the story ends
        if (oxygenBar != null)
        {
            oxygenBar.SetActive(true);
            Debug.Log("Oxygen bar shown via GameObject");
        }

        // ✅ Show oxygen bar via PlayerOxygen script
        PlayerOxygen oxygenSystem = FindObjectOfType<PlayerOxygen>();
        if (oxygenSystem != null)
        {
            oxygenSystem.ShowOxygenBar();
            Debug.Log("Oxygen bar shown via PlayerOxygen script");
        }

        // ✅ Show first objective after story ends
        ShowObjective("Find a way out of the house");

        Debug.Log("Subtitle sequence complete - movement should now be enabled");
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

    // --- Objective Methods ---
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