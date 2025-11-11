using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WordPuzzle : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentWordText;
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    public string correctWord = "PHIVOLCS";
    public List<Button> letterButtons;

    private string currentWord = "";

    public GameObject quizPanel, barrier, quizButton;

    // store original ColorBlocks so ResetWord can fully restore visuals
    private Dictionary<Button, ColorBlock> originalColorBlocks = new Dictionary<Button, ColorBlock>();





    //WATER PUZZLE
    public Objectives_water objectivesWater;
    public GameObject objectToManipulate;

    void Start()
    {
        if (quizButton != null) quizButton.SetActive(false);
        if (quizPanel != null) quizPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        // cache original ColorBlocks and bind listeners that capture the actual button
        foreach (Button btn in letterButtons)
        {
            if (btn == null) continue;

            originalColorBlocks[btn] = btn.colors;

            // capture the current button and its letter in local variables
            string letter = btn.GetComponentInChildren<TextMeshProUGUI>().text;
            Button capturedBtn = btn;

            // pass both the letter and the specific button to the handler
            capturedBtn.onClick.AddListener(() => OnLetterClick(capturedBtn, letter));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ResetWord();
        }
    }

    public void OpenQuiz()
    {
        if (quizPanel != null) quizPanel.SetActive(true);
        if (quizButton != null) quizButton.SetActive(false);
        ResetWord();
        if (feedbackText != null) feedbackText.text = "";
    }

    // Now receives the exact clicked button + letter
    public void OnLetterClick(Button clickedButton, string letter)
    {
        if (clickedButton == null) return;

        // disable only the button that was clicked
        clickedButton.interactable = false;

        // optionally update its appearance to "used"
        ColorBlock colors = clickedButton.colors;
        Color usedColor = new Color(0.5f, 0.5f, 0.5f); // gray
        colors.normalColor = usedColor;
        colors.highlightedColor = usedColor;
        colors.pressedColor = usedColor;
        colors.selectedColor = usedColor;
        clickedButton.colors = colors;

        // Add letter to current word
        currentWord += letter;
        if (currentWordText != null) currentWordText.text = currentWord;

        // Check if player reached full word
        if (currentWord.Length == correctWord.Length)
        {
            CheckAnswer();
        }
    }

    void CheckAnswer()
    {
        if (currentWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            if (feedbackText != null) feedbackText.text = "✅ Correct!";
            DisableButtons();

            if(objectivesWater != null)
            {
                objectivesWater.breakerTurnedOFF = true;
                quizButton.SetActive(false);
                objectToManipulate.GetComponent<Outline>().enabled = false;
                objectToManipulate.GetComponent<BoxCollider>().enabled = false;
            }

            StartCoroutine(CorrectAnswer(1.5f));
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "❌ Try again!";
            // small delay so player sees the "Try again" text (optional)
            StartCoroutine(ResetAfterDelay(0.5f));
        }
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetWord();
        if (feedbackText != null) feedbackText.text = "";
    }

    public void ResetWord()
    {
        currentWord = "";
        if (currentWordText != null) currentWordText.text = "";

        foreach (Button btn in letterButtons)
        {
            if (btn == null) continue;

            btn.interactable = true;

            // restore the entire original ColorBlock (if we saved one)
            if (originalColorBlocks.TryGetValue(btn, out ColorBlock original))
            {
                btn.colors = original;
            }
            else
            {
                // fallback: set default white
                ColorBlock colors = btn.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = Color.white;
                colors.pressedColor = Color.white;
                colors.selectedColor = Color.white;
                btn.colors = colors;
            }
        }
    }

    void DisableButtons()
    {
        foreach (Button btn in letterButtons)
            if (btn != null) btn.interactable = false;
    }

    public void Close()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        ResetWord();
        if (feedbackText != null) feedbackText.text = "";
    }

    private IEnumerator CorrectAnswer(float duration)
    {
        if (currentWordText != null) currentWordText.color = Color.green;
        yield return new WaitForSeconds(duration);
        if (quizPanel != null) quizPanel.SetActive(false);
        if (barrier != null) barrier.SetActive(false);
        AudioManager.Instance.PlaySFX(22);
        DataManager.Instance.factsDiscovered++;
    }
}