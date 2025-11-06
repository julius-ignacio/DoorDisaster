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

    void Start()
    {
        objectiveStage = 0;

        // Initialize tracking for each item
        foreach (ItemPickup item in essentialItems)
        {
            if (item != null)
            {
                itemsCollected[item.itemName] = false;
            }
        }
    }

    public void StartPackingObjective()
    {
        objectiveStage = 1;

        if (subtitleManager != null)
        {
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
                    // Show hint (removed the IsSubtitleActive check that might block hints)
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
        // Debug log to verify item names
        Debug.Log($"Getting hint for item: '{itemName}' (length: {itemName.Length})");

        // Normalize item name to lowercase for comparison
        string itemLower = itemName.ToLower();

        // Health/Medkit hints (matches "First Aid Kit")
        if (itemLower.Contains("health") || itemLower.Contains("medkit") || itemLower.Contains("first aid"))
        {
            return "Maybe I need to check the bathroom cabinet...";
        }

        // Document hints (matches "Documents")
        if (itemLower.Contains("docu") || itemLower.Contains("document") || itemLower.Contains("paper"))
        {
            return "I should check the drawer in the bedroom...";
        }

        // Smartphone/Phone hints (matches "Smartphone")
        if (itemLower.Contains("phone") || itemLower.Contains("smartphone") || itemLower.Contains("mobile"))
        {
            return "Where did I leave my phone? Maybe it's in the bedroom...";
        }

        // Flashlight hints (matches "Flashlight")
        if (itemLower.Contains("flashlight") || itemLower.Contains("flash") || itemLower.Contains("torch"))
        {
            return "I need a flashlight... Maybe there's one in the kitchen cabinet?";
        }

        // Battery hints (matches "Battery")
        if (itemLower.Contains("battery") || itemLower.Contains("batteries"))
        {
            return "I should look for batteries in the kitchen cabinet...";
        }

        // Canned food hints (matches "Canned Food")
        if (itemLower.Contains("canned") || itemLower.Contains("food") || itemLower.Contains("can"))
        {
            return "There should be some canned food in the kitchen cabinet...";
        }

        // Generic hint if no specific match
        return "I should search the house more carefully for essential items...";
    }

    public void OnItemPickedUp(string itemName, bool isEssential)
    {
        if (objectiveStage == 1 && isEssential)
        {
            essentialItemsCollected++;

            // Mark item as collected
            if (itemsCollected.ContainsKey(itemName))
            {
                itemsCollected[itemName] = true;
                Debug.Log($"Item marked as collected: {itemName}");
            }

            if (essentialItems.Length > 0)
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

            // Stop hint system
            if (hintCoroutine != null)
            {
                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }

            subtitleManager?.ShowCustomMessage(
                "I have everything I need. Now I have to get out of here!",
                2f,
                () =>
                {
                    subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
                }
            );
        }
    }

    public int GetObjectiveStage()
    {
        return objectiveStage;
    }

    // Optional: Manual hint trigger (can be called from other scripts)
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