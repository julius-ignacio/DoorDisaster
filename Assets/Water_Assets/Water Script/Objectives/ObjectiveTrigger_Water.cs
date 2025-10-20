using UnityEngine;

public class ObjectiveTrigger_Water : MonoBehaviour
{
    [Header("Link to Objective Manager")]
    public ObjectiveManager_Water objectiveManager;

    [Header("Trigger Settings")]
    public string requiredObjectiveName; // e.g. "Go to Basement"

    private void Start()
    {
        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager_Water>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Directly mark the specified objective complete
            objectiveManager?.CompleteObjective(requiredObjectiveName);
            Debug.Log("✅ Objective completed: " + requiredObjectiveName);

            Destroy(gameObject); // Remove trigger after completing
        }
    }
}
