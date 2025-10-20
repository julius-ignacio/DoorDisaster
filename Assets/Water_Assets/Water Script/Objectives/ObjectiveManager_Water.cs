using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectiveManager_Water : MonoBehaviour
{
    [Header("UI Reference")]
    public TextMeshProUGUI objectiveText;

    [Header("Finish UI")]
    [Tooltip("Panel that appears when all objectives are complete.")]
    public GameObject finishUI; // Assign your 'You Escaped' UI here

    [Header("Objectives List")]
    public List<string> objectives = new List<string>();

    [Header("Options")]
    public bool hideWhenComplete = false;

    [Header("Inventory Reference")]
    public InventoryManager_Water inventoryManager;

    [Header("Item Tracker Reference")]
    public ObjectiveItemTracker_Water itemTracker;

    private HashSet<int> completedObjectives = new HashSet<int>();
    private bool levelFinished = false;

    void Start()
    {
        if (finishUI != null)
            finishUI.SetActive(false); // hide at start

        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager_Water>();

        if (itemTracker == null)
            itemTracker = FindObjectOfType<ObjectiveItemTracker_Water>();

        UpdateUI();
    }

    void Update()
    {
        // ✅ Auto-check if all collectible items have been picked up
        if (itemTracker != null && !IsObjectiveCompleted("Collect All Items"))
        {
            if (itemTracker.IsCompleted())
            {
                CompleteObjective("Collect All Items");
                Debug.Log("📦 Objective auto-completed: Collect All Items");
            }
        }
    }

    public void CompleteObjective(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogWarning("⚠️ Tried to complete an empty or null objective name!");
            return;
        }

        name = name.Trim();
        int index = objectives.FindIndex(obj => obj.Trim().Equals(name, System.StringComparison.OrdinalIgnoreCase));

        if (index == -1)
        {
            Debug.LogWarning($"❌ Objective '{name}' not found! Check spelling or capitalization in the list.");
            Debug.Log("👉 Objectives in manager: " + string.Join(", ", objectives));
            return;
        }

        if (!completedObjectives.Contains(index))
        {
            completedObjectives.Add(index);
            Debug.Log($"✅ Objective completed: {objectives[index]}");
            UpdateUI();

            // 🧩 Check if all are done
            if (completedObjectives.Count == objectives.Count)
                OnAllObjectivesCompleted();
        }
    }

    public bool IsObjectiveCompleted(string name)
    {
        int index = objectives.FindIndex(obj => obj.Trim().Equals(name, System.StringComparison.OrdinalIgnoreCase));
        return index != -1 && completedObjectives.Contains(index);
    }

    private void UpdateUI()
    {
        if (objectiveText == null) return;

        string display = "Objectives:\n";

        for (int i = 0; i < objectives.Count; i++)
        {
            bool done = completedObjectives.Contains(i);
            string objName = objectives[i].Trim().ToLower();

            // Always hide special objectives when done
            if (done && (objName == "turn off the breaker" || objName == "collect all items"))
                continue;

            if (hideWhenComplete && done)
                continue;

            if (done)
                display += $"<color=green>{objectives[i]} (Complete)</color>\n";
            else
                display += $"{objectives[i]}\n";
        }

        objectiveText.text = display.TrimEnd();
        objectiveText.ForceMeshUpdate();
    }

    private void OnAllObjectivesCompleted()
    {
        if (levelFinished) return;
        levelFinished = true;

        Debug.Log("🎉 All objectives completed! You escaped!");

        // ✅ Show the finish screen
        if (finishUI != null)
            finishUI.SetActive(true);

        // Optional: stop game movement or pause
        Time.timeScale = 0f;
    }
}
