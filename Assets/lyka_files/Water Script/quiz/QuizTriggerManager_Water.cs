using UnityEngine;

public class QuizTriggerManager_Water : MonoBehaviour
{
    [Header("Assign all quiz triggers here")]
    public GameObject[] quizTriggers;

    private bool breakerOff = false;

    void Start()
    {
        // Hide all quiz triggers at the start
        foreach (GameObject trigger in quizTriggers)
        {
            trigger.SetActive(false);
        }
    }

    // Call this when the breaker is turned off
    public void ActivateQuizzes()
    {
        if (breakerOff) return; // prevent re-activation

        breakerOff = true;

        foreach (GameObject trigger in quizTriggers)
        {
            trigger.SetActive(true);
        }

        Debug.Log("Breaker turned off — all quiz triggers activated!");
    }
}
