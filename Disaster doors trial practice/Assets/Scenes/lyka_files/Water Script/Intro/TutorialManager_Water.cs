using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager_Water : MonoBehaviour
{
    [Header("Main Containers")]
    public GameObject tutorialPanel;   // The main panel that contains the entire tutorial UI
    public GameObject body1;           // First slide of the tutorial
    public GameObject body2;           // Second slide of the tutorial
    public GameObject body3;           // Third slide of the tutorial

    [Header("Body1 Components")]
    public TextMeshProUGUI headerLine; // Title text for the first slide
    public TextMeshProUGUI body1Text;  // Main body text for the first slide

    [Header("Body2 Components")]
    public TextMeshProUGUI header2;    // Title text for the second slide
    public TextMeshProUGUI body2Text;  // Subtitle or introductory text for the second slide
    public Transform linesContainer;   // Parent object that holds each line of the "game tips"
    public GameObject linePrefab;      // Prefab used to create new bullet-point lines dynamically

    public Image healthMeterFill;      // Image representing the health meter fill (color + value)
    public Image oxygenMeterFill;      // Image representing the oxygen meter fill (color + value)
    public Slider healthSlider;        // UI slider showing health
    public Slider oxygenSlider;        // UI slider showing oxygen

    [Header("Body3 Components")]
    public TextMeshProUGUI header3;    // Title text for the third slide
    public TextMeshProUGUI body3Text;  // Description text for the third slide
    public Transform linesBody3Container; // Container for any extra bullet points (objectives)
    public GameObject lineBody3Prefab;    // Prefab for creating new objective lines

    [Header("Navigation Buttons")]
    public Button prevBtn;             // Button to go to the previous tutorial slide
    public Button nextBtn;             // Button to go to the next tutorial slide
    public TextMeshProUGUI nextBtnText; // The label/text displayed on the "Next" button

    [Header("Settings")]
    public bool showOnStart = true;    // If true, tutorial opens automatically at start
    public float meterAnimationSpeed = 0.5f; // Speed for animating health/oxygen bars

    private int currentSlide = 0;      // Keeps track of which slide is currently being shown
    private GameObject[] bodySlides;   // Array to easily manage all slide GameObjects

    void Start()
    {
        // Store all slide references in an array for easier navigation
        bodySlides = new GameObject[] { body1, body2, body3 };

        // Add event listeners to the navigation buttons
        if (nextBtn != null)
            nextBtn.onClick.AddListener(NextSlide);
        if (prevBtn != null)
            prevBtn.onClick.AddListener(PreviousSlide);

        // If the tutorial should show immediately when the game starts
        if (showOnStart)
        {
            ShowTutorial();
        }
        else
        {
            // Hide tutorial if not showing automatically
            tutorialPanel.SetActive(false);

            // Initialize default meter values (these are visuals only)
            if (healthSlider != null)
                healthSlider.value = 85f;
            if (oxygenSlider != null)
                oxygenSlider.value = 65f;

            if (healthMeterFill != null)
                healthMeterFill.fillAmount = 0.85f;
            if (oxygenMeterFill != null)
                oxygenMeterFill.fillAmount = 0.65f;
        }
    }

    public void ShowTutorial()
    {
        // Activate the tutorial panel and freeze the game
        tutorialPanel.SetActive(true);
        currentSlide = 0;
        Time.timeScale = 0f; // Pauses game time so the player can focus on tutorial

        SetupSlideContent();   // Set up text and visuals for each slide
        DisplaySlide(currentSlide); // Show the first slide
    }

    void SetupSlideContent()
    {
        // ---------- SLIDE 1 CONTENT ----------
        if (headerLine != null)
            headerLine.text = "Trial of Flood: The Flood Rises\n Will You Survive?";

        if (body1Text != null)
            body1Text.text = "The flood is coming!\n Collect all the items before the next water rise.\n Watch your oxygen, health, and timer.\n move fast and reach the safe area to escape.";

        // ---------- SLIDE 2 CONTENT ----------
        if (header2 != null)
            header2.text = "Game Tips:";

        if (body2Text != null)
            body2Text.text = "[ Pay attention to these indicators during the fire emergency! ]";

        // Clear any previous tips already inside the container (prevent duplicates)
        if (linesContainer != null)
        {
            foreach (Transform child in linesContainer)
            {
                if (child.gameObject != linePrefab)
                    Destroy(child.gameObject);
            }
        }

        // Array of helpful game tips to be displayed dynamically
        string[] body2Tips = new string[]
        {
           "• The Oxygen Meter shows how long you can stay underwater.",
           "• The Heart Meter shows your health — avoid drowning or debris.",
           "• The Timer shows when the water will rise again.",
           "• Wrong quiz answers remove 5 seconds, so answer carefully.",
           "• Find keys to unlock doors and access new areas.",
           "• Collect all items and reach the safe zone before it’s too late."
        };

        // Generate each bullet point tip and display in the UI
        if (linePrefab != null && linesContainer != null)
        {
            linePrefab.SetActive(false); // Keep the prefab hidden as a template

            foreach (string tip in body2Tips)
            {
                // Clone the prefab and attach it to the container
                GameObject newLine = Instantiate(linePrefab, linesContainer);
                newLine.SetActive(true);

                // Get the text component to modify its content
                TextMeshProUGUI lineText = newLine.GetComponent<TextMeshProUGUI>();
                if (lineText == null)
                    lineText = newLine.GetComponentInChildren<TextMeshProUGUI>();

                if (lineText != null)
                    lineText.text = "• " + tip; // Add bullet symbol
            }
        }

        // Set colors for health and oxygen meters (visual feedback)
        if (healthMeterFill != null)
            healthMeterFill.color = new Color(0.56f, 1f, 0.56f); // light green
        if (oxygenMeterFill != null)
            oxygenMeterFill.color = new Color(0.31f, 0.47f, 1f); // light blue

        // ---------- SLIDE 3 CONTENT ----------
        if (header3 != null)
            header3.text = "Objectives:";

        if (body3Text != null)
            body3Text.text = "The flood is rising quickly.\n Collect all the needed items before the timer runs out.\n Some doors are locked — find the keys to open them.\n Be careful when answering quizzes; wrong answers remove 5 seconds from your time.\nStay alert, manage your oxygen and health, and reach the safe zone to survive.";

        // Clear old objective lines if any exist
        if (linesBody3Container != null)
        {
            foreach (Transform child in linesBody3Container)
            {
                Destroy(child.gameObject);
            }
        }
    }

    void DisplaySlide(int index)
    {
        // Hide all slides first
        foreach (GameObject slide in bodySlides)
        {
            if (slide != null)
                slide.SetActive(false);
        }

        // Show only the current slide
        if (index >= 0 && index < bodySlides.Length && bodySlides[index] != null)
        {
            bodySlides[index].SetActive(true);

            // If showing the second slide, start animating health/oxygen bars
            if (index == 1)
            {
                StartCoroutine(AnimateMeters());
            }
        }

        // Only show the "Previous" button if we’re not on the first slide
        if (prevBtn != null)
            prevBtn.gameObject.SetActive(index > 0);

        // Change the text on the "Next" button depending on which slide we’re on
        if (nextBtnText != null)
            nextBtnText.text = (index < bodySlides.Length - 1) ? "Next" : "Start Game";
    }

    IEnumerator AnimateMeters()
    {
        // Animate both the health and oxygen meters visually
        float targetHealth = 85f;
        float targetOxygen = 65f;

        // Simulate showing partially filled meters for demonstration
        if (healthSlider != null)
            healthSlider.value = targetHealth;
        if (oxygenSlider != null)
            oxygenSlider.value = targetOxygen;

        if (healthMeterFill != null)
            healthMeterFill.fillAmount = 0.85f;
        if (oxygenMeterFill != null)
            oxygenMeterFill.fillAmount = 0.65f;

        yield return null; // This coroutine could be expanded later for smooth animation
    }

    void NextSlide()
    {
        // When "Next" is clicked, go to the next slide or close if it’s the last one
        if (currentSlide < bodySlides.Length - 1)
        {
            currentSlide++;
            DisplaySlide(currentSlide);
        }
        else
        {
            CloseTutorial(); // Exit tutorial when the last slide is reached
        }
    }

    void PreviousSlide()
    {
        // Go back to the previous slide
        if (currentSlide > 0)
        {
            currentSlide--;
            DisplaySlide(currentSlide);
        }
    }

    void CloseTutorial()
    {
        // Hide the tutorial panel and resume the game
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Unpauses the game
    }
}
