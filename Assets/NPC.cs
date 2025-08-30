using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject npc, detectionPlane, helpBtn;

    

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
        }
    }

    public void OnButtonClick()
    {
        Debug.Log("Help button clicked!");
        npc.SetActive(false);
    }
}
