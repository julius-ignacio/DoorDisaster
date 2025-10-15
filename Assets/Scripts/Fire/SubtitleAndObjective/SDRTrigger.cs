using UnityEngine;

public class SDRTrigger : MonoBehaviour
{
    [Header("References")]
    public StopDropRoll stopDropRollScript;
    public DoorFireTrigger doorFireTrigger; // Check if door objective was shown
    public SubtitleManager2 subtitleManager; // To update objective after SDR
    public FireSafetyQuiz quizManager; // Quiz manager for SDR quiz

    [Header("Settings")]
    public bool triggerOnce = true; // Only trigger fire once

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // Check if player entered the fire
        if (other.CompareTag("Player") && !hasTriggered)
        {
            // Check if the door fire message has been shown (prerequisite)
            if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
            {
                Debug.Log("Player tried to trigger SDR but hasn't seen the door fire yet");
                return; // Don't trigger SDR yet
            }

            // Trigger the Stop Drop Roll sequence
            if (stopDropRollScript != null)
            {
                stopDropRollScript.TriggerOnFire();
                Debug.Log("Player caught fire - SDR sequence started!");

                // Wait for SDR to complete, then show quiz
                StartCoroutine(WaitForSDRAndShowQuiz());

                if (triggerOnce)
                    hasTriggered = true;
            }
            else
            {
                Debug.LogError("StopDropRoll script reference is missing!");
            }
        }
    }

    System.Collections.IEnumerator WaitForSDRAndShowQuiz()
    {
        // Wait until SDR is complete (you could also make StopDropRoll call back here)
        // For now, estimate based on timings: 2s warning + drop time + 3 rolls + 2s completion message
        yield return new WaitForSeconds(10f); // Adjust based on your actual SDR duration

        // Show the quiz
        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("stop_drop_roll");
        if (quiz != null && quizManager != null)
        {
            quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () => {
                // After quiz completes, show objective
                if (subtitleManager != null)
                    subtitleManager.ShowObjective("Try the window in the bedroom");
            });
        }
        else
        {
            Debug.LogError("Quiz 'stop_drop_roll' not found or quizManager not assigned!");
            // Fallback: show objective without quiz
            if (subtitleManager != null)
                subtitleManager.ShowObjective("Try the window in the bedroom");
        }
    }

    // Optional: Visualize the fire trigger in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange transparent
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}