using System;
using Narrate;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject npc, detectionPlane, helpBtn, QuizUI;
    public int valueToFireForQuizSys;

    public QuizScript quizScript;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            helpBtn.SetActive(true);
            quizScript.selected = valueToFireForQuizSys;
            quizScript.ChangeTexts(); // <-- This actually updates the UI
            Debug.Log("Player entered NPC trigger. Selected value: " + quizScript.selected);
                    }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            helpBtn.SetActive(false);
            QuizUI.SetActive(false);
        }
    }

    public void OnButtonClick()
    {
        Debug.Log("Help button clicked!");
        npc.SetActive(false);
        QuizUI.SetActive(true);
    }
}
