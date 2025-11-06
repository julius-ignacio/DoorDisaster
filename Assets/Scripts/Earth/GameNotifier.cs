using MilkShake;
using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class GameNotifier : MonoBehaviour
{
    public GameObject gameTextNotify;
    //public QuizScript quizScript; // assign in Inspector
    void Start()
    {
        gameTextNotify.SetActive(false);
    }


    public void EarnedPoints(int points, float duration = 3f)
    {
        if (points > 1)
        {
            StartCoroutine(ShowNotificationCoroutine($"+{points} Points Earned!", duration));

        }

        else
        {
            StartCoroutine(ShowNotificationCoroutine($"+{points} Point Earned!", duration));
        }
    }

    
        public void ObtainedItem(int points, string itemName, float duration = 3f)
    {
        StartCoroutine(ShowNotificationCoroutine($"{itemName} obtained. You earned {points} point!", duration));
    }

public void PanicWarning(float duration = 3f)
    {
        StartCoroutine(ShowNotificationCoroutine("Panic meter critical! Stay calm — you've got this!", duration));
    }


    public void BarrierRemoved(float duration = 5f)
    {
        StartCoroutine(ShowNotificationCoroutine("Objectives completed. You can now get through the barrier and reach the exit.", duration));
    }

    public void HelmetBreak(float duration = 10f)
    {
        StartCoroutine(ShowNotificationCoroutine(
            "⚠️ Your safety helmet has broken! Stay alert — falling debris can be dangerous.",
            duration
        ));
    }


    public void cantHeal_FullHealth(float duration = 5f)
    {
        StartCoroutine(ShowNotificationCoroutine(
            "Health is full. Medkit stored in backpack.",
            duration
        ));
    }




public void notInPanic(float duration = 5f)
{
    StartCoroutine(ShowNotificationCoroutine(
        "Not in panic.",
        duration
    ));
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
