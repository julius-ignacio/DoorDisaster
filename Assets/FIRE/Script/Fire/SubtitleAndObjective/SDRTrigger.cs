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
    
    // ✅ Static flag for persistence across saves/restarts
    public static bool SDRTriggered { get; private set; } = false;

    void Start()
    {
        // ✅ Restore state from static flag
        hasTriggered = SDRTriggered;
        
        if (hasTriggered)
        {
            Debug.Log("✅ SDRTrigger restored: Already triggered, won't fire again");
            
            // ✅ Restore the objective if SDR was already done
            if (subtitleManager != null)
            {
                Invoke(nameof(RestoreSDRObjective), 0.2f);
            }
        }
    }

    private void RestoreSDRObjective()
    {
        // ✅ Show the bedroom window objective
        if (subtitleManager != null)
        {
            subtitleManager.ShowObjective("Try the window in the bedroom");
            Debug.Log("✅ Restored SDR objective: Try window");
        }
    }

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
                Debug.Log("🔥 Player caught fire - SDR sequence started!");
                
                // Listen for SDR completion
                stopDropRollScript.OnSDRComplete = () =>
                {
                    ShowQuizAfterSDR();
                };
                
                if (triggerOnce)
                {
                    hasTriggered = true;
                    SDRTriggered = true; // ✅ Update static flag
                    Debug.Log("🔥 SDR triggered - flag set, won't trigger again");
                }
            }
            else
            {
                Debug.LogError("❌ StopDropRoll script reference is missing!");
            }
        }
    }
    
    private void ShowQuizAfterSDR()
    {
        Debug.Log("🔥 SDR complete - showing quiz...");
        
        QuizQuestion2 quiz = QuizDatabase2.GetQuiz("stop_drop_roll");
        if (quiz != null && quizManager != null)
        {
            quizManager.ShowQuiz(quiz.question, quiz.answers, quiz.correctAnswerIndex, () =>
            {
                Debug.Log("🔥 SDR quiz completed - showing window objective");
                
                // ✅ Show objective after quiz
                if (subtitleManager != null)
                {
                    subtitleManager.ShowObjective("Try the window in the bedroom");
                    Debug.Log("✅ Objective shown: Try the window in the bedroom");
                }
                else
                {
                    Debug.LogError("❌ SubtitleManager is null!");
                }
            });
        }
        else
        {
            Debug.LogError("❌ Quiz 'stop_drop_roll' not found or quizManager not assigned!");
            
            // Fallback: show objective without quiz
            if (subtitleManager != null)
            {
                subtitleManager.ShowObjective("Try the window in the bedroom");
                Debug.Log("✅ Fallback objective shown: Try the window in the bedroom");
            }
        }
    }

    // ✅ Public method for save system
    public static void RestoreSDRState(bool triggered)
    {
        SDRTriggered = triggered;
        Debug.Log($"🔥 Restored SDR state: triggered={triggered}");
    }

    // ✅ Reset on new game
    public static void ResetSDRProgress()
    {
        SDRTriggered = false;
        Debug.Log("🔥 SDR progress reset");
    }
    
    // Optional: Visualize the fire trigger in editor
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange transparent
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}