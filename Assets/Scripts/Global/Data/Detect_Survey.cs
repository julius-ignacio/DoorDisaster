using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Detect_Survey : MonoBehaviour
{
    public GameObject AnswerBtn;

    void Start()
    {
        AnswerBtn.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AnswerBtn.SetActive(true);
        }
    }
    

       private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           AnswerBtn.SetActive(false);
        }
    }

}
