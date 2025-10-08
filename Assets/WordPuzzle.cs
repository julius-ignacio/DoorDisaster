using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WordPuzzle : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentWordText;
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    public string correctWord = "PHIVOLCS";
    public List<Button> letterButtons;

    private string currentWord = "";

    public GameObject quizPanel;

    void Start()
    {
        quizPanel.SetActive(false);
        feedbackText.text = "";
        foreach (Button btn in letterButtons)
        {
            string letter = btn.GetComponentInChildren<TextMeshProUGUI>().text; // assumes button text is the letter
            btn.onClick.AddListener(() => OnLetterClick(letter));
        }
    }

    public void OnLetterClick(string letter)
    {
        currentWord += letter;
        currentWordText.text = currentWord;

        // Check if player reached word length
        if (currentWord.Length == correctWord.Length)
        {
            CheckAnswer();
        }
    }

    void CheckAnswer()
    {
        if (currentWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            feedbackText.text = "✅ Correct!";
            // You can trigger next question or scene here
            DisableButtons();
        }
        else
        {
            feedbackText.text = "❌ Try again!";
            ResetWord();
        }
    }

    public void ResetWord()
    {
        currentWord = "";
        currentWordText.text = "";
    }

    void DisableButtons()
    {
        foreach (Button btn in letterButtons)
            btn.interactable = false;
    }


    public void Close()
    {
        quizPanel.SetActive(false);
        ResetWord();
        feedbackText.text = "";
    }
}
