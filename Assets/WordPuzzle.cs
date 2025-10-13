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


    public void OpenQuiz()
    {
        quizPanel.SetActive(true);
        quizButton.SetActive(false);
        ResetWord();
        feedbackText.text = "";
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
