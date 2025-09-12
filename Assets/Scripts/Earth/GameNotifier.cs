using MilkShake;
using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class GameNotifier : MonoBehaviour
{
    public GameObject gameTextNotify;
    public QuizScript quizScript; // assign in Inspector
    void Start()
    {
        gameTextNotify.SetActive(false);
    }


    public void EarnedPoints(int points, float duration = 3f)
    {
        StartCoroutine(ShowNotificationCoroutine($"+{points} Erudition Points Earned!", duration));
    }

public void PanicWarning(float duration = 3f)
{
    StartCoroutine(ShowNotificationCoroutine("Panic meter critical! Stay calm — you've got this!", duration));
}






    public void ShowNotification(string message, float duration = 3f)
    {
        StartCoroutine(ShowNotificationCoroutine(message, duration));
    }

private IEnumerator ShowNotificationCoroutine(string message, float duration)
{
    gameTextNotify.SetActive(true);
    gameTextNotify.GetComponent<TMP_Text>().text = message;

    yield return new WaitForSeconds(duration);

    gameTextNotify.SetActive(false);
}

}
