using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class NPC_GiveId_water : MonoBehaviour
{
    public int NpcId;
    public QuizScript_water quizScript;

    public GameObject quizUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizScript.currentNpcId = NpcId; // Pass the NPC ID to QuizScript
            quizScript.helpBtn.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizUI.SetActive(false);
            quizScript.helpBtn.SetActive(false);
         }
    }
}
