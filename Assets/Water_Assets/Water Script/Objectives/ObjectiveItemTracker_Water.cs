using UnityEngine;

public class ObjectiveItemTracker_Water : MonoBehaviour
{
    [Header("Objective Manager Reference")]
    public ObjectiveManager_Water objectiveManager;

    [Header("Objective Name")]
    public string objectiveName = "Collect All Items";

    [Header("References")]
    public InventoryManager_Water inventoryManager;

    private int totalCollects;
    private int collected = 0;
    private bool completed = false;

    private void Start()
    {
        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager_Water>();

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager_Water>();

        // Count only active "Collect" items in the scene
        totalCollects = 0;
        foreach (var obj in GameObject.FindGameObjectsWithTag("Collect"))
        {
            if (obj.activeInHierarchy)
                totalCollects++;
        }

        Debug.Log($"🎒 Found {totalCollects} Collect items in the scene.");

        if (totalCollects == 0)
            Debug.LogWarning("⚠️ No Collect items found in the scene!");
    }

    public void RegisterPickup(string tag)
    {
        if (completed) return;

        // Ignore special items that shouldn’t count
        if (tag == "Flashlight" || tag == "bag" || tag == "Bandage" || tag == "Battery" || tag.EndsWith("Key"))
            return;

        collected++;
        Debug.Log($"📦 Item collected: {collected}/{totalCollects}");

        // ✅ Update inventory count
        if (inventoryManager != null)
            inventoryManager.AddItem();

        CheckIfComplete();
    }

    private void CheckIfComplete()
    {
        if (completed) return;
        if (totalCollects == 0) return; // avoid divide-by-zero cases

        if (collected >= totalCollects)
        {
            completed = true;
            Debug.Log("✅ All Collect items collected — objective complete!");
            objectiveManager?.CompleteObjective(objectiveName);
        }
    }

    public void ResetProgress()
    {
        collected = 0;
        completed = false;

        if (inventoryManager != null)
            inventoryManager.totalItems = totalCollects;
    }

    // ✅ Used by ObjectiveManager_Water to check completion
    public bool IsCompleted()
    {
        return completed;
    }
}
