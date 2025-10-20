using UnityEngine;
using System;

public class EventFloodSystem_Water : MonoBehaviour
{
    // --- Events ---
    public static event Action OnFloodStart;
    public static event Action OnFloodEnd;
    public static event Action OnQuizTriggered;
    public static event Action<bool> OnQuizCompleted; // bool = wasCorrect

    private static bool floodActive = false;

    // --- Methods to trigger events ---
    public static void StartFlood()
    {
        if (floodActive) return;
        floodActive = true;
        Debug.Log("🌊 Flood started!");
        OnFloodStart?.Invoke();
    }

    public static void EndFlood()
    {
        if (!floodActive) return;
        floodActive = false;
        Debug.Log("✅ Flood ended!");
        OnFloodEnd?.Invoke();
    }

    public static void TriggerQuiz()
    {
        Debug.Log("📝 Quiz triggered!");
        OnQuizTriggered?.Invoke();
    }

    public static void CompleteQuiz(bool correct)
    {
        Debug.Log($"Quiz completed! Correct: {correct}");
        OnQuizCompleted?.Invoke(correct);
    }
}
