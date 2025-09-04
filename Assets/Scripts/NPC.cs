using System;
using HMStudio.EasyQuiz;
using Narrate;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject npc, detectionPlane, helpBtn, QuizUI;
    public string quizFileName; // e.g. "Quiz_NPC1.xlsx"
    public QuestionManager questionManager;

    void Start()
    {
        QuizUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            helpBtn.SetActive(true);
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

        // Load this NPC’s quiz
        questionManager.LoadQuizFile(quizFileName);
    }
}
