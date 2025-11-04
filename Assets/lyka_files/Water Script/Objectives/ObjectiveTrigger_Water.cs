using UnityEngine;

public class ObjectiveTrigger_Water : MonoBehaviour
{
    [Header("Objective Settings")]
    [Tooltip("The objective to complete when this trigger is activated")]
    public string objectiveToComplete;

    [Tooltip("The next objective to add after completing (optional)")]
    public string nextObjectiveToAdd;

    [Header("Trigger Type")]
    [Tooltip("How this trigger activates")]
    public TriggerType triggerType = TriggerType.OnInteract;

    [Header("References")]
    public ObjectiveManager_Water objectiveManager;

    [Header("One-Time Trigger")]
    [Tooltip("Can this trigger only be activated once?")]
    public bool oneTimeOnly = true;

    private bool hasTriggered = false;

    public enum TriggerType
    {
        OnInteract,      // When player presses interact key
        OnCollision,     // When player enters trigger collider
        OnCall           // When called manually from another script
    }

    private void Start()
    {
        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager_Water>();
    }

    // For interact-based triggers (like breaker switches)
    public void TriggerObjective()
    {
        if (oneTimeOnly && hasTriggered)
        {
            Debug.Log($"⚠️ Objective trigger '{objectiveToComplete}' already activated.");
            return;
        }

        hasTriggered = true;
        CompleteAndAdvance();
    }

    // For collision-based triggers (entering rooms)
    private void OnTriggerEnter(Collider other)
    {
        if (triggerType != TriggerType.OnCollision) return;
        if (!other.CompareTag("Player")) return;

        if (oneTimeOnly && hasTriggered)
        {
            Debug.Log($"⚠️ Objective trigger '{objectiveToComplete}' already activated.");
            return;
        }

        hasTriggered = true;
        CompleteAndAdvance();
    }

    private void CompleteAndAdvance()
    {
        if (objectiveManager == null)
        {
            Debug.LogError("❌ ObjectiveManager not found!");
            return;
        }

        // Complete the current objective
        if (!string.IsNullOrEmpty(objectiveToComplete))
        {
            objectiveManager.CompleteMainObjective(objectiveToComplete);
            Debug.Log($"✅ Objective completed via trigger: {objectiveToComplete}");
        }

        // Add the next objective
        if (!string.IsNullOrEmpty(nextObjectiveToAdd))
        {
            objectiveManager.AddMainObjective(nextObjectiveToAdd);
            Debug.Log($"🆕 Next objective added: {nextObjectiveToAdd}");
        }
    }

    // For manual calls from other scripts
    public void ManualTrigger()
    {
        TriggerObjective();
    }

    [ContextMenu("Test Trigger")]
    private void TestTrigger()
    {
        hasTriggered = false; // Reset for testing
        TriggerObjective();
    }
}