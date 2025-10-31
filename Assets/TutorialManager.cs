using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Main Containers")]
    public GameObject tutorialPanel; // Intro_Panel_Game_Guide
    public GameObject body1; // Body1 - First slide
    public GameObject body2; // Body2 - Second slide (with icons and meters)
    public GameObject body3; // Body3 - Third slide

    [Header("Body1 Components")]
    public TextMeshProUGUI headerLine; // HeaderLine text
    public TextMeshProUGUI body1Text; // body1 text

    [Header("Body2 Components")]
    public TextMeshProUGUI header2; // header2 text
    public TextMeshProUGUI body2Text; // body2 text (for tip header)
    public Transform linesContainer; // Lines container for bullet points
    public GameObject linePrefab; // Single line prefab to duplicate

    // Meters - can be either Image fills or Sliders
    public Image healthMeterFill; // Health bar fill image (inside slider's Fill Area)
    public Image oxygenMeterFill; // Oxygen bar fill image (inside slider's Fill Area)

    // Optional: if you want to animate the slider values instead
    public UnityEngine.UI.Slider healthSlider; // Optional: the slider component itself
    public UnityEngine.UI.Slider oxygenSlider; // Optional: the slider component itself

    [Header("Body3 Components")]
    public TextMeshProUGUI header3; // header3 text
    public TextMeshProUGUI body3Text; // body3 text (for objectives intro)
    public Transform linesBody3Container; // Lines container in Body3
    public GameObject lineBody3Prefab; // Line prefab for body3

    [Header("Navigation Buttons")]
    public Button prevBtn; // PrevBtn
    public Button nextBtn; // NextBtn
    public TextMeshProUGUI nextBtnText; // Text component of NextBtn

    [Header("Settings")]
    public bool showOnStart = true;
    public float meterAnimationSpeed = 0.5f;

    private int currentSlide = 0;
    private GameObject[] bodySlides;

    void Start()
    {
        // Store all body slides
        bodySlides = new GameObject[] { body1, body2, body3 };

        // Setup button listeners
        if (nextBtn != null)
            nextBtn.onClick.AddListener(NextSlide);
        if (prevBtn != null)
            prevBtn.onClick.AddListener(PreviousSlide);

        if (showOnStart)
        {
            ShowTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);

            // ✅ Set default values for 0-100 scale
            if (healthSlider != null)
                healthSlider.value = 85f;  // 85 out of 100
            if (oxygenSlider != null)
                oxygenSlider.value = 65f;  // 65 out of 100

            // Fill images use 0-1 scale
            if (healthMeterFill != null)
                healthMeterFill.fillAmount = 0.85f;
            if (oxygenMeterFill != null)
                oxygenMeterFill.fillAmount = 0.65f;
        }
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        currentSlide = 0;
        Time.timeScale = 0f; // Pause the game

        SetupSlideContent();
        DisplaySlide(currentSlide);
    }

    void SetupSlideContent()
    {
        // === SLIDE 1: Introduction ===
        if (headerLine != null)
            headerLine.text = "Trial of Fire: Escape the Inferno — Will You Survive?";

        if (body1Text != null)
            body1Text.text = "A fire has broken out in your home! Navigate through smoke and flames, rescue Mr. Kitty, and find your way to safety. Remember your fire safety training — your survival depends on it. Stay calm and think fast!";

        // === SLIDE 2: Game Tips ===
        if (header2 != null)
            header2.text = "Game Tips:";

        if (body2Text != null)
            body2Text.text = "[ Pay attention to these indicators during the fire emergency! ]";

        // Clear existing lines in Body2
        if (linesContainer != null)
        {
            foreach (Transform child in linesContainer)
            {
                if (child.gameObject != linePrefab)
                    Destroy(child.gameObject);
            }
        }

        // Create bullet points for Body2
        string[] body2Tips = new string[]
        {
            "The <color=#90FF90>Health</color> bar shows your physical condition — getting too close to fire or touching hot surfaces will damage you.",
            "The <color=#5078FF>Oxygen</color> bar depletes over time in smoke-filled areas. Find oxygen canisters throughout the house to refill it.",
            "Hot doors can burn you! Find protection like cloth or wet towels before touching them.",
            "Fire extinguishers use the PASS method — you'll need to answer questions to activate them.",
            "Some doors are locked — search for keys quickly when under pressure.",
            "If you catch fire, use Stop, Drop, and Roll to extinguish the flames!"
        };

        if (linePrefab != null && linesContainer != null)
        {
            linePrefab.SetActive(false); // Hide the template

            foreach (string tip in body2Tips)
            {
                GameObject newLine = Instantiate(linePrefab, linesContainer);
                newLine.SetActive(true);
                TextMeshProUGUI lineText = newLine.GetComponent<TextMeshProUGUI>();
                if (lineText == null)
                    lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();

                if (lineText != null)
                    lineText.text = "• " + tip;
            }
        }

        // === SLIDE 3: Objectives ===
        if (header3 != null)
            header3.text = "Objectives:";

        if (body3Text != null)
            body3Text.text = "Your mission has four critical phases. Complete them all to escape alive:";

        // Clear existing lines in Body3
        if (linesBody3Container != null)
        {
            foreach (Transform child in linesBody3Container)
            {
                if (child.gameObject != lineBody3Prefab)
                    Destroy(child.gameObject);
            }
        }

        // Create objectives for Body3
        string[] objectives = new string[]
        {
            "<b>Phase 1 - Initial Response:</b> Wake up in your bedroom, call emergency services, protect your hand from the hot door, and turn off the fuse box.",
            "<b>Phase 2 - Cat Rescue:</b> Find a wet towel, teleport to House B, use a fire extinguisher to clear a path, find the key, and rescue Mr. Kitty from the locked bedroom.",
            "<b>Phase 3 - Escape Attempt:</b> Gather essential items, deal with blocked exits, use Stop-Drop-Roll if you catch fire, and break the window to escape.",
            "<b>Phase 4 - Final Challenge:</b> Sprint through the hallway as fire chases you, reach the barrier, pass the final fire safety quiz, and escape to freedom!"
        };

        if (lineBody3Prefab != null && linesBody3Container != null)
        {
            lineBody3Prefab.SetActive(false); // Hide the template

            foreach (string objective in objectives)
            {
                GameObject newLine = Instantiate(lineBody3Prefab, linesBody3Container);
                newLine.SetActive(true);
                TextMeshProUGUI lineText = newLine.GetComponent<TextMeshProUGUI>();
                if (lineText == null)
                    lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();

                if (lineText != null)
                    lineText.text = "• " + objective;
            }
        }

        // Set meter colors to match your bars
        if (healthMeterFill != null)
            healthMeterFill.color = new Color(0.56f, 1f, 0.56f); // Bright green like your image

        if (oxygenMeterFill != null)
            oxygenMeterFill.color = new Color(0.31f, 0.47f, 1f); // Blue like your image
    }

    void DisplaySlide(int index)
    {
        // Hide all slides
        foreach (GameObject slide in bodySlides)
        {
            if (slide != null)
                slide.SetActive(false);
        }

        // Show current slide
        if (index >= 0 && index < bodySlides.Length && bodySlides[index] != null)
        {
            bodySlides[index].SetActive(true);

            // Animate meters on slide 2 (Body2)
            if (index == 1)
            {
                StartCoroutine(AnimateMeters());
            }
        }

        // Update button states
        if (prevBtn != null)
            prevBtn.gameObject.SetActive(index > 0);

        if (nextBtnText != null)
            nextBtnText.text = (index < bodySlides.Length - 1) ? "Next" : "Start Game";
    }

    IEnumerator AnimateMeters()
    {
        // ✅ Updated for 0-100 scale (not 0-1)
        float targetHealth = 85f;  // 85% of 100
        float targetOxygen = 65f;  // 65% of 100

        // Set values immediately (no animation)
        if (healthSlider != null)
            healthSlider.value = targetHealth;
        if (oxygenSlider != null)
            oxygenSlider.value = targetOxygen;

        // Fill images still use 0-1 scale
        if (healthMeterFill != null)
            healthMeterFill.fillAmount = 0.85f;
        if (oxygenMeterFill != null)
            oxygenMeterFill.fillAmount = 0.65f;

        yield return null;
    }

    void NextSlide()
    {
        if (currentSlide < bodySlides.Length - 1)
        {
            currentSlide++;
            DisplaySlide(currentSlide);
        }
        else
        {
            CloseTutorial();
        }
    }

    void PreviousSlide()
    {
        if (currentSlide > 0)
        {
            currentSlide--;
            DisplaySlide(currentSlide);
        }
    }

    void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game

        // Optional: Trigger game start event
        // GameManager.Instance.StartGame();
    }
}