using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
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

    void Start()
    {
        quizButton.SetActive(false);
        quizPanel.SetActive(false);
        feedbackText.text = "";
        foreach (Button btn in letterButtons)
        {
            string letter = btn.GetComponentInChildren<TextMeshProUGUI>().text; // assumes button text is the letter
            btn.onClick.AddListener(() => OnLetterClick(letter));
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
        quizPanel.SetActive(true);
        quizButton.SetActive(false);
        ResetWord();
        feedbackText.text = "";
    }
public void OnLetterClick(string letter)
{
    // Find which button was clicked (based on letter)
    Button clickedButton = letterButtons.Find(b => 
        b.GetComponentInChildren<TextMeshProUGUI>().text == letter && b.interactable);

    if (clickedButton != null)
    {
        // Gray out and disable the clicked button
        clickedButton.interactable = false;

        ColorBlock colors = clickedButton.colors;
        colors.normalColor = new Color(0.5f, 0.5f, 0.5f); // gray
        colors.highlightedColor = colors.normalColor;
        colors.pressedColor = colors.normalColor;
        colors.selectedColor = colors.normalColor;
        clickedButton.colors = colors;
    }

    // Add letter to current word
    currentWord += letter;
    currentWordText.text = currentWord;

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
            feedbackText.text = "✅ Correct!";
            // You can trigger next question or scene here
            DisableButtons();

            StartCoroutine(CorrectAnswer(1.5f));
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

    foreach (Button btn in letterButtons)
    {
        btn.interactable = true;

        ColorBlock colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        btn.colors = colors;
    }
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


    private IEnumerator CorrectAnswer(float duration)
    {
        currentWordText.color = Color.green;
        yield return new WaitForSeconds(duration);
        quizPanel.SetActive(false);
        barrier.SetActive(false);
        AudioManager.Instance.PlaySFX(22);
        DataManager.Instance.factsDiscovered ++;
    }
}
