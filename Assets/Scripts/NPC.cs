using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int npcId;
    public GameObject helpBtn;
    public GameObject quizUI;
    public QuizScript quizScript;

    void Start()
    {
        quizUI.SetActive(false);
        helpBtn.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            helpBtn.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            helpBtn.SetActive(false);
            quizUI.SetActive(false);
        }
    }

    public void OnHelpButtonClick()
    {
        helpBtn.SetActive(false);
        quizUI.SetActive(true);

        List<QuizQuestion> questions = null;
        switch (npcId)
        {
            case 1: questions = QuizDatabase.NPC1; break;
            case 2: questions = QuizDatabase.NPC2; break;
            case 3: questions = QuizDatabase.NPC3; break;
            case 4: questions = QuizDatabase.NPC4; break;
        }

        if (questions != null)
            quizScript.BeginQuiz(questions);
    }
}
