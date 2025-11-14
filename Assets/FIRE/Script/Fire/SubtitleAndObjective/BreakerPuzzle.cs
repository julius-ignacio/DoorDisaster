using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BreakerPuzzle : MonoBehaviour
{
    [Header("UI References")]
    public GameObject breakerPanel;
    public Button bedroomSwitch;
    public Button comfortSwitch;
    public Button kitchenSwitch;
    public Button livingSwitch;

    [Header("Text References")]
    public TextMeshProUGUI errorText;
    [Tooltip("Assign the TextMeshProUGUI INSIDE the BedroomSwitch button (NOT BedroomLabel)")]
    public TextMeshProUGUI bedroomButtonText;
    [Tooltip("Assign the TextMeshProUGUI INSIDE the ComfortSwitch button (NOT ComfortLabel)")]
    public TextMeshProUGUI comfortButtonText;
    [Tooltip("Assign the TextMeshProUGUI INSIDE the KitchenSwitch button (NOT KitchenLabel)")]
    public TextMeshProUGUI kitchenButtonText;
    [Tooltip("Assign the TextMeshProUGUI INSIDE the LivingSwitch button (NOT LivingLabel)")]
    public TextMeshProUGUI livingButtonText;

    [Header("Lighting")]
    public Light[] houseLights;

    [Header("Quiz Manager")]
    public FireSafetyQuiz quizManager;

    [Header("UI Elements to Hide")]
    public SubtitleManager2 subtitleManager;
    public GameObject healthBar;
    public GameObject oxygenBar;

    [Header("Audio")]
    public int switchClickSFX = 38;
    public int switchErrorSFX = 39;

    [Header("Settings")]
    public Color onColor = Color.green;
    public Color offColor = Color.gray;
    public float resetDelay = 1.5f;

    private bool bedroomOff = false;
    private bool comfortOff = false;
    private bool kitchenOff = false;
    private bool livingOff = false;
    private bool puzzleComplete = false;

    public static bool BreakerPuzzleComplete { get; private set; } = false;

    void Start()
    {
        if (breakerPanel != null)
            breakerPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        bedroomSwitch.onClick.AddListener(OnBedroomClicked);
        comfortSwitch.onClick.AddListener(OnComfortClicked);
        kitchenSwitch.onClick.AddListener(OnKitchenClicked);
        livingSwitch.onClick.AddListener(OnLivingClicked);

        InitializeButtonVisuals();

        if (errorText != null)
            errorText.text = "";

        // ✅ If puzzle was already completed, restore the lights-off state
        if (BreakerPuzzleComplete)
        {
            puzzleComplete = true;
            DimLights();
            Debug.Log("✅ BreakerPuzzle restored: Puzzle already complete, lights turned off");
        }

        Debug.Log($"BreakerPuzzle.Start(): BreakerPuzzleComplete={BreakerPuzzleComplete}");
    }

    // ✅ Public method for save system
    public static void RestoreBreakerState(bool complete)
    {
        BreakerPuzzleComplete = complete;
        Debug.Log($"🔌 Restored breaker state: complete={complete}");
    }

    // ✅ Reset on new game
    public static void ResetBreakerProgress()
    {
        BreakerPuzzleComplete = false;
        Debug.Log("🔌 Breaker progress reset");
    }

    public void ShowPuzzle()
    {
        if (breakerPanel != null)
            breakerPanel.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (subtitleManager != null)
            subtitleManager.HideObjective();

        if (healthBar != null)
            healthBar.SetActive(false);

        if (oxygenBar != null)
            oxygenBar.SetActive(false);
    }

    void OnBedroomClicked()
    {
        if (puzzleComplete) return;

        if (!bedroomOff)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(switchClickSFX);

            bedroomOff = true;
            UpdateButtonVisual(bedroomSwitch, bedroomButtonText, true);
            ShowError("Bedroom breaker OFF", Color.green);
            Debug.Log("Bedroom breaker turned off");
        }
    }

    void OnComfortClicked()
    {
        if (puzzleComplete) return;

        if (!comfortOff)
        {
            if (!bedroomOff)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(switchErrorSFX);

                ShowError("Turn off Bedroom first!", Color.red);
                StartCoroutine(ResetSwitchesAfterDelay());
                return;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(switchClickSFX);

            comfortOff = true;
            UpdateButtonVisual(comfortSwitch, comfortButtonText, true);
            ShowError("Comfort Room breaker OFF", Color.green);
            Debug.Log("Comfort room breaker turned off");
        }
    }

    void OnKitchenClicked()
    {
        if (puzzleComplete) return;

        if (!kitchenOff)
        {
            if (!bedroomOff || !comfortOff)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(switchErrorSFX);

                ShowError("Follow order: Bedroom → Comfort → Kitchen → Living", Color.red);
                StartCoroutine(ResetSwitchesAfterDelay());
                return;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(switchClickSFX);

            kitchenOff = true;
            UpdateButtonVisual(kitchenSwitch, kitchenButtonText, true);
            ShowError("Kitchen breaker OFF", Color.green);
            Debug.Log("Kitchen breaker turned off");
        }
    }

    void OnLivingClicked()
    {
        if (puzzleComplete) return;

        if (!livingOff)
        {
            if (!bedroomOff || !comfortOff || !kitchenOff)
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlaySFX(switchErrorSFX);

                ShowError("Follow order: Bedroom → Comfort → Kitchen → Living", Color.red);
                StartCoroutine(ResetSwitchesAfterDelay());
                return;
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(switchClickSFX);

            livingOff = true;
            UpdateButtonVisual(livingSwitch, livingButtonText, true);
            ShowError("Living Room breaker OFF", Color.green);
            Debug.Log("Living room breaker turned off");

            puzzleComplete = true;
            BreakerPuzzleComplete = true;
            StartCoroutine(CompletePuzzle());
        }
    }

    void UpdateButtonVisual(Button button, TextMeshProUGUI buttonText, bool isOff)
    {
        Image buttonImage = button.GetComponent<Image>();

        if (isOff)
        {
            buttonImage.color = offColor;
            if (buttonText != null)
                buttonText.text = "OFF";
        }
        else
        {
            buttonImage.color = onColor;
            if (buttonText != null)
                buttonText.text = "ON";
        }
    }

    void ShowError(string message, Color color)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = color;
        }
    }

    IEnumerator ResetSwitchesAfterDelay()
    {
        yield return new WaitForSecondsRealtime(resetDelay);
        ResetAllSwitches();
        ShowError("Switches reset. Try again!", Color.yellow);
        Debug.Log("All switches reset to ON");
    }

    void InitializeButtonVisuals()
    {
        if (bedroomSwitch != null)
        {
            Image img = bedroomSwitch.GetComponent<Image>();
            if (img != null) img.color = onColor;
            if (bedroomButtonText != null) bedroomButtonText.text = "ON";
        }

        if (comfortSwitch != null)
        {
            Image img = comfortSwitch.GetComponent<Image>();
            if (img != null) img.color = onColor;
            if (comfortButtonText != null) comfortButtonText.text = "ON";
        }

        if (kitchenSwitch != null)
        {
            Image img = kitchenSwitch.GetComponent<Image>();
            if (img != null) img.color = onColor;
            if (kitchenButtonText != null) kitchenButtonText.text = "ON";
        }

        if (livingSwitch != null)
        {
            Image img = livingSwitch.GetComponent<Image>();
            if (img != null) img.color = onColor;
            if (livingButtonText != null) livingButtonText.text = "ON";
        }
    }

    void ResetAllSwitches()
    {
        bedroomOff = false;
        comfortOff = false;
        kitchenOff = false;
        livingOff = false;

        UpdateButtonVisual(bedroomSwitch, bedroomButtonText, false);
        UpdateButtonVisual(comfortSwitch, comfortButtonText, false);
        UpdateButtonVisual(kitchenSwitch, kitchenButtonText, false);
        UpdateButtonVisual(livingSwitch, livingButtonText, false);
    }

    IEnumerator CompletePuzzle()
    {
        yield return new WaitForSecondsRealtime(2f);

        DimLights();

        if (breakerPanel != null)
            breakerPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (healthBar != null)
            healthBar.SetActive(true);

        if (oxygenBar != null)
            oxygenBar.SetActive(true);

        Debug.Log("Breaker puzzle complete - lights dimmed, game resumed");

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Okay, power's off. Now I need to find a wet towel - I can't breathe with all this smoke!",
                4f,
                () => {
                    ShowWetTowelQuiz();
                }
            );
        }
    }

    void ShowWetTowelQuiz()
    {
        Debug.Log("BreakerPuzzle: Showing wet towel quiz...");

        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("wet_towel");

        if (quiz != null && quizManager != null)
        {
            Debug.Log("BreakerPuzzle: Quiz found, displaying...");

            quizManager.ShowQuiz(
                quiz.question,
                quiz.answers,
                quiz.correctAnswerIndex,
                () => {
                    if (subtitleManager != null)
                    {
                        subtitleManager.ShowObjective("Find a wet towel in the bathroom");
                    }
                }
            );
        }
        else
        {
            Debug.LogError("Quiz 'wet_towel' not found in database or QuizManager not assigned!");

            if (subtitleManager != null)
            {
                subtitleManager.ShowObjective("Find a wet towel in the bathroom");
            }
        }
    }

    void DimLights()
    {
        if (houseLights == null || houseLights.Length == 0)
        {
            Debug.LogWarning("No lights assigned to breaker puzzle!");
            return;
        }

        foreach (Light light in houseLights)
        {
            if (light != null)
                light.intensity = 0f;
        }

        Debug.Log("Lights turned off completely");
    }
}