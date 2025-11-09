using System.Collections;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public MrKittyPickup mrKittyPickup;

    [Header("Essential Items")]
    public ItemPickup[] essentialItems;

    private int objectiveStage = 0;
    private int essentialItemsCollected = 0;

    void Start()
    {
        objectiveStage = 0;
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
                    }
                    else
                    {
                        subtitleManager.ShowObjective("Find the nearest exit and escape");
                    }
                }
            );
        }
    }

    public void OnItemPickedUp(string itemName, bool isEssential)
    {
        if (objectiveStage == 1 && isEssential)
        {
            essentialItemsCollected++;

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

            subtitleManager?.ShowCustomMessage(
                "I have everything I need. Now I have to get out of here!",
                2f,
                () =>
                {
                    subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
                    // 🔥 Fire trigger will happen when player re-enters door zone
                }
            );
        }
    }

    public int GetObjectiveStage()
    {
        return objectiveStage;
    }
}
