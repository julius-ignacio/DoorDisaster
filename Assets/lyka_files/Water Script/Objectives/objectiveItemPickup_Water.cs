using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveItemPickup_Water : MonoBehaviour
{
    [Header("References")]
    public ObjectiveManager_Water objectiveManager;
    public InventoryManager_Water inventoryManager;
    public ObjectiveItemTracker_Water itemTracker;

    [Header("Pickup Settings")]
    [Tooltip("Name of the objective to complete when this item is picked up.")]
    public string objectiveToComplete;

    [Tooltip("Item name to add to the inventory when picked up. Leave empty to auto-detect by GameObject name.")]
    public string itemName = "";

    [Tooltip("Should this item be destroyed after pickup?")]
    public bool destroyOnPickup = true;

    [Header("Debug")]
    public bool logDebugInfo = true;

    private void Start()
    {
        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager_Water>();

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager_Water>();

        if (itemTracker == null)
            itemTracker = FindObjectOfType<ObjectiveItemTracker_Water>();

        // Make sure the collider is set as trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"⚠️ {gameObject.name}: Collider is NOT set as trigger! Setting it now.");
            col.isTrigger = true;
        }

        if (logDebugInfo)
        {
            Debug.Log($"🔍 {gameObject.name} pickup script initialized. Tag: {tag}, ItemName: {(string.IsNullOrEmpty(itemName) ? "AUTO-DETECT" : itemName)}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (logDebugInfo)
        {
            Debug.Log($"🔍 {gameObject.name}: Trigger entered by {other.gameObject.name} (Tag: {other.tag})");
        }

        if (!other.CompareTag("Player"))
        {
            if (logDebugInfo)
                Debug.Log($"❌ {gameObject.name}: Not player, ignoring.");
            return;
        }

        Debug.Log($"✅ {gameObject.name}: PLAYER DETECTED! Processing pickup...");

        // 🧩 Auto-detect item name if not manually set
        if (string.IsNullOrEmpty(itemName))
        {
            // Use GameObject name as item name
            itemName = gameObject.name;
            Debug.Log($"🔍 Auto-detected item name: {itemName}");
        }

        // 🧹 Clean up any clone suffix or extra spacing
        itemName = itemName.Replace("(Clone)", "").Trim();
        Debug.Log($"📝 Final item name after cleanup: '{itemName}'");

        // ✅ REGISTER WITH ITEM TRACKER FIRST (this adds to inventory too)
        if (itemTracker != null)
        {
            Debug.Log($"📦 Calling itemTracker.RegisterPickup('{itemName}')");
            itemTracker.RegisterPickup(itemName);
            Debug.Log($"✅ RegisterPickup completed for: {itemName}");
        }
        else if (inventoryManager != null)
        {
            // Fallback if no tracker (shouldn't happen)
            Debug.LogWarning($"⚠️ No ItemTracker found - adding '{itemName}' directly to inventory");
            inventoryManager.AddItem(itemName);
        }
        else
        {
            Debug.LogError($"❌ CRITICAL: No InventoryManager or ItemTracker found in scene for {itemName}!");
            return;
        }

        // ✅ Complete individual objective if set (like "Unlock Office Door")
        if (!string.IsNullOrEmpty(objectiveToComplete) && objectiveManager != null)
        {
            objectiveManager.CompleteObjective(objectiveToComplete);
            Debug.Log($"🎯 Objective completed: {objectiveToComplete}");
        }

        // ✅ Disable or remove the item
        if (destroyOnPickup)
        {
            Debug.Log($"💥 Destroying: {gameObject.name}");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"👻 Hiding: {gameObject.name}");
            gameObject.SetActive(false);
        }
    }

    // Manual test method
    [ContextMenu("Test Pickup (Simulate Player Touch)")]
    private void TestPickup()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("🧪 TEST: Simulating player pickup...");
            OnTriggerEnter(player.GetComponent<Collider>());
        }
        else
        {
            Debug.LogError("❌ TEST FAILED: No GameObject with 'Player' tag found!");
        }
    }
}