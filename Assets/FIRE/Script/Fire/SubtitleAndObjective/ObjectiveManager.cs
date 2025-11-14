using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public MrKittyPickup mrKittyPickup;

    [Header("Essential Items")]
    public ItemPickup[] essentialItems;

    [Header("Hint Settings")]
    [Tooltip("Time in seconds before showing first hint")]
    public float hintDelay = 20f;
    [Tooltip("Time between repeated hints if player still hasn't found items")]
    public float hintRepeatDelay = 30f;

    private int objectiveStage = 0;
    private int essentialItemsCollected = 0;
    private Dictionary<string, bool> itemsCollected = new Dictionary<string, bool>();
    private Coroutine hintCoroutine;

    // ✅ Track if we're loading from save to suppress duplicate messages
    private bool isLoadingFromSave = false;

    // ✅ Static property for objective stage persistence
    public static int SavedObjectiveStage { get; private set; } = 0;

    void Start()
    {
        // Initialize tracking for each item
        foreach (ItemPickup item in essentialItems)
        {
            if (item != null)
            {
                itemsCollected[item.itemName] = false;
            }
        }

        // ✅ Check if we're loading from a save file
        var dm = DataManager.Instance;
        if (dm != null && WorldSaveSystem.HasSaveData(dm.currentTrial, dm.currentMode))
        {
            isLoadingFromSave = true;
            Debug.Log("✅ ObjectiveManager: Loading from save, restoring progress");

            // ✅ Restore objective stage first
            objectiveStage = SavedObjectiveStage;
            Debug.Log($"✅ Restored objective stage: {objectiveStage}");

            // ✅ Restore collected items from ItemPickup's static data
            RestoreCollectedItems();

            // ✅ Show the correct objective based on stage
            RestoreObjectiveUI();

            // Re-enable messages after a brief delay
            StartCoroutine(ReenableMessagesAfterLoad());
        }
        else
        {
            // ✅ Fresh start - explicitly reset stage
            objectiveStage = 0;
            SavedObjectiveStage = 0;
            Debug.Log("✅ ObjectiveManager: Fresh start, stage = 0");
        }
    }

    private void RestoreCollectedItems()
    {
        // Get all picked up item IDs from the static ItemPickup data
        string[] pickedUpIDs = ItemPickup.GetPickedUpItems();

        essentialItemsCollected = 0;

        // Check which essential items were already picked up
        foreach (ItemPickup item in essentialItems)
        {
            if (item != null)
            {
                bool wasPickedUp = System.Array.Exists(pickedUpIDs, id => id == item.uniqueItemID);

                if (wasPickedUp)
                {
                    itemsCollected[item.itemName] = true;
                    essentialItemsCollected++;
                    Debug.Log($"✅ Restored item: {item.itemName} ({essentialItemsCollected}/{essentialItems.Length})");
                }
            }
        }

        Debug.Log($"✅ Restored progress: {essentialItemsCollected}/{essentialItems.Length} items collected");
    }

    private void RestoreObjectiveUI()
    {
        if (subtitleManager == null) return;

        // ✅ Check if door fire was triggered - it overrides stage 2 objective
        if (DoorFireTrigger.FireMessageShown)
        {
            // Fire blocks the door - show alternative escape objective
            // The DoorFireTrigger will restore this objective in its Start()
            Debug.Log("Fire triggered - DoorFireTrigger will restore alternative escape objective");
            return; // Let DoorFireTrigger handle the objective
        }

        // Show the correct objective based on stage
        if (objectiveStage == 0)
        {
            // No objective yet (before packing starts)
            Debug.Log("Stage 0: No objective shown yet");
        }
        else if (objectiveStage == 1)
        {
            // Still collecting items
            if (essentialItems.Length > 0)
            {
                subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");
                Debug.Log($"Stage 1: Showing collect items objective ({essentialItemsCollected}/{essentialItems.Length})");
            }
        }
        else if (objectiveStage >= 2)
        {
            // All items collected - escape objective
            subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
            Debug.Log("Stage 2+: Showing escape objective");
        }
    }

    private IEnumerator ReenableMessagesAfterLoad()
    {
        yield return new WaitForSeconds(0.5f);
        isLoadingFromSave = false;
        Debug.Log("✅ ObjectiveManager: Re-enabled pickup messages for new items");
    }

    public void StartPackingObjective()
    {
        objectiveStage = 1;
        SavedObjectiveStage = 1; // ✅ Save stage

        if (subtitleManager != null)
        {
            // ✅ Don't show intro message if we already have items
            if (essentialItemsCollected > 0)
            {
                // Just update the objective, skip the intro dialogue
                if (essentialItems.Length > 0)
                {
                    subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");
                }

                // Start hint system if we still need items
                if (essentialItemsCollected < essentialItems.Length)
                {
                    if (hintCoroutine != null)
                        StopCoroutine(hintCoroutine);
                    hintCoroutine = StartCoroutine(HintSystem());
                }
            }
            else
            {
                // Fresh start - show intro message
                subtitleManager.ShowCustomMessage(
                    "I need to pack my essentials and get out of here...",
                    2f,
                    () =>
                    {
                        if (essentialItems.Length > 0)
                        {
                            subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");

                            // Start hint system
                            if (hintCoroutine != null)
                                StopCoroutine(hintCoroutine);
                            hintCoroutine = StartCoroutine(HintSystem());
                        }
                        else
                        {
                            subtitleManager.ShowObjective("Find the nearest exit and escape");
                        }
                    }
                );
            }
        }
    }

    private IEnumerator HintSystem()
    {
        // Wait before first hint
        yield return new WaitForSeconds(hintDelay);

        while (objectiveStage == 1 && essentialItemsCollected < essentialItems.Length)
        {
            // Find which items haven't been collected yet
            List<string> missingItems = new List<string>();

            foreach (var item in itemsCollected)
            {
                if (!item.Value)
                {
                    missingItems.Add(item.Key);
                }
            }

            if (missingItems.Count > 0)
            {
                // Pick a random missing item to hint about
                string randomMissingItem = missingItems[Random.Range(0, missingItems.Count)];
                string hint = GetHintForItem(randomMissingItem);

                if (!string.IsNullOrEmpty(hint))
                {
                    if (subtitleManager != null)
                    {
                        subtitleManager.ShowCustomMessage(hint, 3f, null);
                        Debug.Log($"Hint shown for: {randomMissingItem}");
                    }
                }
            }

            // Wait before next hint
            yield return new WaitForSeconds(hintRepeatDelay);
        }
    }

    private string GetHintForItem(string itemName)
    {
        Debug.Log($"Getting hint for item: '{itemName}' (length: {itemName.Length})");

        string itemLower = itemName.ToLower();

        if (itemLower.Contains("health") || itemLower.Contains("medkit") || itemLower.Contains("first aid"))
        {
            return "Maybe I need to check the bathroom cabinet...";
        }

        if (itemLower.Contains("docu") || itemLower.Contains("document") || itemLower.Contains("paper"))
        {
            return "I should check the drawer in the bedroom...";
        }

        if (itemLower.Contains("phone") || itemLower.Contains("smartphone") || itemLower.Contains("mobile"))
        {
            return "Where did I leave my phone? Maybe it's in the bedroom...";
        }

        if (itemLower.Contains("flashlight") || itemLower.Contains("flash") || itemLower.Contains("torch"))
        {
            return "I need a flashlight... Maybe there's one in the kitchen cabinet?";
        }

        if (itemLower.Contains("battery") || itemLower.Contains("batteries"))
        {
            return "I should look for batteries in the kitchen cabinet...";
        }

        if (itemLower.Contains("canned") || itemLower.Contains("food") || itemLower.Contains("can"))
        {
            return "There should be some canned food in the kitchen cabinet...";
        }

        return "I should search the house more carefully for essential items...";
    }

    public void OnItemPickedUp(string itemName, bool isEssential)
    {
        if (objectiveStage == 1 && isEssential)
        {
            // ✅ Prevent duplicate counting (important for save/load)
            if (itemsCollected.ContainsKey(itemName) && itemsCollected[itemName])
            {
                Debug.Log($"⚠️ Item '{itemName}' already counted, ignoring");
                return;
            }

            essentialItemsCollected++;

            // Mark item as collected
            if (itemsCollected.ContainsKey(itemName))
            {
                itemsCollected[itemName] = true;
                Debug.Log($"Item marked as collected: {itemName}");
            }

            // ✅ Only show subtitle if NOT loading from save
            if (!isLoadingFromSave && essentialItems.Length > 0)
            {
                subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");
            }

            if (essentialItemsCollected >= essentialItems.Length)
            {
                AllEssentialsCollected();
            }
        }
    }

    private void AllEssentialsCollected()
    {
        if (objectiveStage == 1)
        {
            objectiveStage = 2;
            SavedObjectiveStage = 2; // ✅ Save stage transition

            // Stop hint system
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }

            // ✅ Only show completion message if NOT loading from save
            if (!isLoadingFromSave)
            {
                subtitleManager?.ShowCustomMessage(
                    "I have everything I need. Now I have to get out of here!",
                    2f,
                    () =>
                    {
                        subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
                    }
                );
            }
            else
            {
                // Just update objective silently
                subtitleManager?.ShowObjective("Find the nearest exit and escape the fire");
            }
        }
    }

    public int GetObjectiveStage()
    {
        return objectiveStage;
    }

    // ✅ Public method for save system
    public static void RestoreObjectiveStage(int stage)
    {
        SavedObjectiveStage = stage;
        Debug.Log($"📋 Restored objective stage: {stage}");
    }

    // ✅ Reset on new game
    public static void ResetObjectiveProgress()
    {
        SavedObjectiveStage = 0;
        Debug.Log("📋 Objective progress reset to stage 0");
    }

    public void ShowHintForMissingItems()
    {
        List<string> missingItems = new List<string>();

        foreach (var item in itemsCollected)
        {
            if (!item.Value)
            {
                missingItems.Add(item.Key);
            }
        }

        if (missingItems.Count > 0)
        {
            string randomMissingItem = missingItems[Random.Range(0, missingItems.Count)];
            string hint = GetHintForItem(randomMissingItem);

            if (!string.IsNullOrEmpty(hint) && subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(hint, 3f, null);
            }
        }
    }
}