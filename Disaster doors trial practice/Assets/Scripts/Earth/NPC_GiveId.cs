using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class NPC_GiveId : MonoBehaviour
{
    public int NpcId;
    public QuizScript quizScript;

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
