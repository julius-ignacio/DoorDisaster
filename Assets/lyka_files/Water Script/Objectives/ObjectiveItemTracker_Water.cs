using UnityEngine;

public class ObjectiveItemTracker_Water : MonoBehaviour
{
    [Header("Objective Manager Reference")]
    public ObjectiveManager_Water objectiveManager;

    [Header("References")]
    public InventoryManager_Water inventoryManager;

    [Header("Debug Info")]
    [Tooltip("Shows current collection progress in Inspector")]
    public string debugProgress = "Not Started";

    private int totalCollects = 9; // 9 items to collect
    private int collected = 0;
    private bool completed = false;

    // Track which items have been registered to prevent duplicates
    private System.Collections.Generic.HashSet<string> registeredItems = new System.Collections.Generic.HashSet<string>();

    private void Start()
    {
        if (objectiveManager == null)
            objectiveManager = FindObjectOfType<ObjectiveManager_Water>();

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager_Water>();

        // Count items with "Collect" tag
        Debug.Log("=== COUNTING COLLECTIBLE ITEMS ===");
        
        try
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Collect");
            totalCollects = 0;
            
            foreach (var obj in items)
            {
                if (obj.activeInHierarchy)
                {
                    totalCollects++;
                    Debug.Log($"   ✓ Found: {obj.name} (Tag: Collect)");
                }
            }
            
            Debug.Log($"📦 Total 'Collect' tagged items: {totalCollects}");
        }
        catch
        {
            Debug.LogError("❌ Tag 'Collect' not found! Using default count of 9.");
            totalCollects = 9;
        }

        Debug.Log($"🎒 TOTAL COLLECTIBLES TO FIND: {totalCollects}");
        Debug.Log("===================================");

        if (totalCollects == 0)
        {
            Debug.LogWarning("⚠️ NO COLLECTIBLE ITEMS FOUND! Using default count of 9.");
            totalCollects = 9;
        }
        
        // Initialize the progress counter immediately - START AT 0
        UpdateProgressDisplay();
        UpdateDebugInfo();
    }

    public void RegisterPickup(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Debug.LogWarning("⚠️ RegisterPickup called with empty item name!");
            return;
        }

        // Normalize item name (remove spaces and convert to lowercase)
        string normalizedName = itemName.Trim().ToLower().Replace(" ", "");

        Debug.Log($"🧾 Registering pickup: {itemName} (normalized: {normalizedName})");

        // Check if already registered (prevent duplicates)
        if (registeredItems.Contains(normalizedName))
        {
            Debug.LogWarning($"⚠️ Item '{itemName}' was already registered! Skipping duplicate.");
            return;
        }

        // Mark as registered FIRST
        registeredItems.Add(normalizedName);

        // Add item to inventory
        if (inventoryManager != null)
            inventoryManager.AddItem(itemName);

        // Determine item type
        bool isCollectibleItem = IsCollectibleItem(normalizedName);
        bool isKey = IsKeyItem(normalizedName);
        bool isBag = normalizedName == "bag";
        bool isFlashlight = normalizedName == "flashlight";

        // Handle collectible items (count toward the 9 items)
        if (isCollectibleItem && !completed)
        {
            collected++;
            Debug.Log($"📦 COLLECTIBLE PROGRESS: {collected}/{totalCollects} items collected");
            
            // Update the progress counter display
            UpdateProgressDisplay();
            UpdateDebugInfo();
            
            // Check if collection is complete
            CheckIfComplete();
        }
        // Handle keys (trigger objectives)
        else if (isKey)
        {
            Debug.Log($"🔑 Key picked up: {itemName} (does NOT count toward collection)");
            HandleKeyPickup(normalizedName, itemName);
        }
        // Handle bag
        else if (isBag)
        {
            Debug.Log($"🎒 Bag picked up: {itemName}");
        }
        // Handle flashlight
        else if (isFlashlight)
        {
            Debug.Log($"💡 Flashlight picked up: {itemName}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Unknown item type: {itemName} (normalized: {normalizedName})");
        }
    }

    // ✅ FIXED: Match your EXACT GameObject names
    private bool IsCollectibleItem(string normalizedName)
    {
        // Your actual GameObject names (normalized to lowercase, no spaces)
        return normalizedName == "medkit" ||           // Medkit
               normalizedName == "smallradio" ||       // SmallRadio
               normalizedName == "waterbottle" ||      // Water Bottle
               normalizedName == "cannedfood" ||       // Canned Food
               normalizedName == "rope" ||             // Rope
               normalizedName == "flaregun" ||         // Flare Gun
               normalizedName == "ducttape" ||         // Duct Tape
               normalizedName == "documents" ||        // Documents
               normalizedName == "walkietalkie";       // Walkie Talkie (if you have it)
    }

    // Check if the item is a key
    private bool IsKeyItem(string normalizedName)
    {
        return normalizedName.Contains("key");
    }

    // Handle key pickups - NEW SEQUENCE
    private void HandleKeyPickup(string normalizedName, string displayName)
    {
        if (objectiveManager == null) return;

        // Office Key → triggers going to office
        if (normalizedName == "officekey" || normalizedName == "officeroomkey")
        {
            objectiveManager.CompleteMainObjective("Find the Office Room Key");
            objectiveManager.AddMainObjective("Go to the Office Room and collect the Flare Gun, Documents, and Study Room Key");
            Debug.Log("🔑 Office Key picked up - added Office objective");
        }
        // Basement Key → triggers going to basement to turn off breaker
        else if (normalizedName == "basementkey")
        {
            objectiveManager.CompleteMainObjective("Go to the Bedroom");
            objectiveManager.AddMainObjective("Go to the Basement and turn off the Breaker");
            Debug.Log("🔑 Basement Key picked up - added Basement objective");
        }
        // Study Key → triggers going to study room
        else if (normalizedName == "studykey" || normalizedName == "studyroomkey")
        {
            objectiveManager.CompleteMainObjective("Go to the Office Room and collect the Flare Gun, Documents, and Study Room Key");
            objectiveManager.AddMainObjective("Go to the Study Room and collect the Medkit and Parents' Bedroom Key");
            Debug.Log("🔑 Study Room Key picked up - added Study Room objective");
        }
        // Parents' Bedroom Key → triggers going to parents' bedroom
        else if (normalizedName == "parentskey" || normalizedName == "parentsbedroomkey" || normalizedName == "bedroomkey")
        {
            objectiveManager.CompleteMainObjective("Go to the Study Room and collect the Medkit and Parents' Bedroom Key");
            objectiveManager.AddMainObjective("Go to the Parents' Bedroom and collect the Walkie Talkie and Garage Key");
            Debug.Log("🔑 Parents' Bedroom Key picked up - added Parents' Bedroom objective");
        }
        // Garage Key → triggers going to garage
        else if (normalizedName == "garagekey")
        {
            objectiveManager.CompleteMainObjective("Go to the Parents' Bedroom and collect the Walkie Talkie and Garage Key");
            objectiveManager.AddMainObjective("Go to the Garage and collect the Rope, Duct Tape, and Balcony Key");
            Debug.Log("🔑 Garage Key picked up - added Garage objective");
        }
        // Balcony Key → final key, escape when all items collected
        else if (normalizedName == "balconykey")
        {
            objectiveManager.CompleteMainObjective("Go to the Garage and collect the Rope, Duct Tape, and Balcony Key");
            Debug.Log("🔑 Balcony Key picked up - escape will trigger after all items collected");
        }
    }

    // Update the progress display in the ObjectiveManager
    private void UpdateProgressDisplay()
    {
        if (objectiveManager != null && !completed)
        {
            objectiveManager.UpdateObjectiveProgress("Collect All Items", collected, totalCollects);
            Debug.Log($"🔄 Updated UI progress: {collected}/{totalCollects}");
        }
    }

    private void UpdateDebugInfo()
    {
        debugProgress = $"{collected}/{totalCollects} collected";
    }

    private void CheckIfComplete()
    {
        if (completed)
        {
            Debug.LogWarning("⚠️ CheckIfComplete called but already completed!");
            return;
        }
        
        if (totalCollects == 0)
        {
            Debug.LogError("❌ Cannot check completion - totalCollects is 0!");
            return;
        }

        Debug.Log($"🔍 Checking completion: {collected} >= {totalCollects}? {collected >= totalCollects}");

        // Complete when ALL 9 items are collected
        if (collected >= totalCollects)
        {
            completed = true;
            Debug.Log("🎉 ========================================");
            Debug.Log("🎉 ALL ITEMS COLLECTED!");
            Debug.Log($"🎉 Final Count: {collected}/{totalCollects}");
            Debug.Log("🎉 ========================================");
            
            // Complete the collection sub-objective
            objectiveManager?.CompleteSubObjective("Collect All Items");
            
            // Add the final escape objective
            objectiveManager?.AddMainObjective("Go to the Balcony and Escape");
            Debug.Log("🚪 Added final objective: Go to the Balcony and Escape");
        }
        else
        {
            Debug.Log($"⏳ Not complete yet. Need {totalCollects - collected} more items.");
        }
    }

    public void ResetProgress()
    {
        collected = 0;
        completed = false;
        registeredItems.Clear();

        if (inventoryManager != null)
            inventoryManager.totalItems = totalCollects;
        
        UpdateProgressDisplay();
        UpdateDebugInfo();
        
        Debug.Log("🔄 Progress reset!");
    }

    public bool IsCompleted()
    {
        return completed;
    }

    // Debug methods
    [ContextMenu("Debug: Show Current State")]
    private void DebugShowState()
    {
        Debug.Log("=== OBJECTIVE TRACKER STATE ===");
        Debug.Log($"Total Items in Scene: {totalCollects}");
        Debug.Log($"Items Collected: {collected}");
        Debug.Log($"Completed: {completed}");
        Debug.Log($"Progress: {collected}/{totalCollects}");
        Debug.Log($"Registered Items: {string.Join(", ", registeredItems)}");
        Debug.Log("==============================");
    }

    [ContextMenu("Debug: Force Complete Collection")]
    private void DebugForceComplete()
    {
        collected = totalCollects;
        CheckIfComplete();
    }

    [ContextMenu("Debug: Add Test Item")]
    private void DebugAddTestItem()
    {
        RegisterPickup("TestItem");
    }
}