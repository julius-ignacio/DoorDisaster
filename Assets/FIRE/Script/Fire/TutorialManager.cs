using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Icon")]
    public Button tutorialIconBtn;

    [Header("Main Containers")]
    public GameObject tutorialPanel;
    public GameObject body1;
    public GameObject body2;
    public GameObject body3;

    [Header("Body1 Components")]
    public TextMeshProUGUI headerLine;
    public TextMeshProUGUI body1Text;

    [Header("Body2 Components")]
    public TextMeshProUGUI header2;
    public TextMeshProUGUI body2Text;
    public Transform linesContainer;
    public GameObject linePrefab;

    public Image healthMeterFill;
    public Image oxygenMeterFill;
    public Slider healthSlider;
    public Slider oxygenSlider;

    [Header("Body3 Components")]
    public TextMeshProUGUI header3;
    public TextMeshProUGUI body3Text;
    public Transform linesBody3Container;
    public GameObject lineBody3Prefab;

    [Header("Navigation Buttons")]
    public Button prevBtn;
    public Button nextBtn;
    public TextMeshProUGUI nextBtnText;

    [Header("Settings")]
    public bool showOnStart = true;
    public float meterAnimationSpeed = 0.5f;

    private int currentSlide = 0;
    private GameObject[] bodySlides;

    void Start()
    {
        bodySlides = new GameObject[] { body1, body2, body3 };

        if (nextBtn != null)
            nextBtn.onClick.AddListener(NextSlide);
        if (prevBtn != null)
            prevBtn.onClick.AddListener(PreviousSlide);

        if (tutorialIconBtn != null)
            tutorialIconBtn.onClick.AddListener(ShowTutorial);

        var dm = DataManager.Instance;

        // ✅ Skip tutorial if already done
        if (dm != null && dm.playerData != null && dm.playerData.tutorialDone)
        {
            tutorialPanel.SetActive(false);
            return;
        }

        if (showOnStart)
        {
            ShowTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);

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
        tutorialPanel.SetActive(true);
        currentSlide = 0;
        Time.timeScale = 0f;

        SetupSlideContent();
        DisplaySlide(currentSlide);
    }

    void SetupSlideContent()
    {
        // Slide 1
        if (headerLine != null)
            headerLine.text = "Trial of Fire: Escape the Flames";

        if (body1Text != null)
            body1Text.text = "A fire has broken out in your home! Navigate through smoke and flames, rescue Mr. Kitty, and find your way to safety. Remember your fire safety training — your survival depends on it. Stay calm and think fast!";

        // Slide 2
        if (header2 != null)
            header2.text = "Game Tips:";

        if (body2Text != null)
            body2Text.text = "[ Pay attention to these indicators during the fire emergency! ]";

        if (linesContainer != null)
        {
            foreach (Transform child in linesContainer)
            {
                if (child.gameObject != linePrefab)
                    Destroy(child.gameObject);
            }
        }

        string[] body2Tips = new string[]
        {
            "The <color=#90FF90>Health</color> bar shows your physical condition. Getting too close to fire or touching hot surfaces will damage you.",
            "The <color=#5078FF>Oxygen</color> bar depletes over time in smoke-filled areas. Find oxygen canisters throughout the house to refill it.",
            "Hot doors can burn you. Find protection like cloth or wet towels before touching them.",
            "Fire extinguishers use the PASS method. You'll need to answer questions to activate them.",
            "Some doors are locked. Search for keys quickly when under pressure.",
            "If you catch fire, use Stop Drop and Roll to extinguish the flames."
        };

        if (linePrefab != null && linesContainer != null)
        {
            linePrefab.SetActive(false);

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

        if (healthMeterFill != null)
            healthMeterFill.color = new Color(0.56f, 1f, 0.56f);

        if (oxygenMeterFill != null)
            oxygenMeterFill.color = new Color(0.31f, 0.47f, 1f);

        // Slide 3
        if (header3 != null)
            header3.text = "Objectives:";

        if (body3Text != null)
            body3Text.text = "Your journey is guided by key objectives. Each one must be completed to move forward. Pay attention and stay focused. Your progress depends on it. <size=145%><color=#FF4444>BE QUICK</color></size> <color=white>the fire is spreading.</color>";

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
        foreach (GameObject slide in bodySlides)
        {
            if (slide != null)
                slide.SetActive(false);
        }

        if (index >= 0 && index < bodySlides.Length && bodySlides[index] != null)
        {
            bodySlides[index].SetActive(true);

            if (index == 1)
            {
                StartCoroutine(AnimateMeters());
            }
        }

        if (prevBtn != null)
            prevBtn.gameObject.SetActive(index > 0);

        if (nextBtnText != null)
            nextBtnText.text = (index < bodySlides.Length - 1) ? "Next" : "Start Game";
    }

    IEnumerator AnimateMeters()
    {
        float targetHealth = 85f;
        float targetOxygen = 65f;

        if (healthSlider != null)
            healthSlider.value = targetHealth;
        if (oxygenSlider != null)
            oxygenSlider.value = targetOxygen;

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
        Time.timeScale = 1f;

        // ✅ Mark tutorial as done
        var dm = DataManager.Instance;
        if (dm != null && dm.playerData != null)
        {
            dm.playerData.tutorialDone = true;
        }
    }
}
