using System.Collections;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public MrKittyPickup mrKittyPickup;

    [Header("Backpack Settings")]
    public ItemPickup backpackPickup;

    [Header("Essential Items")]
    public ItemPickup[] essentialItems;

    private int objectiveStage = 0;
    private int essentialItemsCollected = 0;
    private bool backpackPickedUp = false;

    void Start()
    {
        // Packing objective only starts after cat pickup
        objectiveStage = 0;
    }

    // Call this from MrKittyPickup after the quiz is complete
    public void StartPackingObjective()
    {
        objectiveStage = 2; // Changed from 1 to 2 so backpack can be picked up immediately

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "I need to pack my essentials and get out of here...",
                2f,
                () =>
                {
                    subtitleManager.ShowObjective("Pick up the backpack from the bedroom");
                }
            );
        }
    }

    // Call this when the player enters the bedroom area
    public void OnBedroomReached()
    {
        if (objectiveStage == 1)
        {
            objectiveStage = 2;
            if (subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(
                    "There's my backpack! Let me grab it.",
                    2f,
                    () =>
                    {
                        subtitleManager.ShowObjective("Pick up the backpack from the bedroom");
                    }
                );
            }
        }
    }

    // Call this from the backpack ItemPickup script or detect it here
    public void OnBackpackPickedUp()
    {
        if (objectiveStage == 2)
        {
            backpackPickedUp = true;
            objectiveStage = 3;

            if (subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(
                    "Good, now let me gather the things I really need to survive...",
                    3f,
                    () =>
                    {
                        if (essentialItems.Length > 0)
                        {
                            subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");
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

    // Called whenever an item is picked up
    public void OnItemPickedUp(string itemName, bool isEssential)
    {
        // If backpack is picked up, move to stage 3
        if (itemName == "Backpack" && objectiveStage == 2)
        {
            OnBackpackPickedUp();
            return;
        }

        if (objectiveStage == 3 && isEssential)
        {
            essentialItemsCollected++;

            if (essentialItems.Length > 0)
            {
                subtitleManager.ShowObjective($"Collect essential items ({essentialItemsCollected}/{essentialItems.Length})");
            }

            // Check if all essential items are collected
            if (essentialItemsCollected >= essentialItems.Length)
            {
                AllEssentialsCollected();
            }
        }
    }

    private void AllEssentialsCollected()
    {
        if (objectiveStage == 3)
        {
            objectiveStage = 4;

            if (subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(
                    "I have everything I need. Now I have to get out of here!",
                    2f,
                    () =>
                    {
                        subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
                    }
                );
            }
        }
    }

    public int GetObjectiveStage()
    {
        return objectiveStage;
    }

    public bool IsBackpackPickedUp()
    {
        return backpackPickedUp;
    }
}