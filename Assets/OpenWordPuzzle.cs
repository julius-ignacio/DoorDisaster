using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.Collections;

public class OpenWordPuzzle : MonoBehaviour
{
    public GameObject quizButton, quizPanel;

    void Start()
    {
        quizButton.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizButton.SetActive(true);
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizButton.SetActive(false);
            quizPanel.SetActive(false);
        }
    }
}
