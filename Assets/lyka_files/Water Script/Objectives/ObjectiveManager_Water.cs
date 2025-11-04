using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveManager_Water : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI mainObjectiveText;
    public TextMeshProUGUI subObjectiveText;
    
    // Legacy property for backward compatibility with EndingTrigger_Water
    public TextMeshProUGUI objectiveText
    {
        get { return mainObjectiveText; }
        set { mainObjectiveText = value; }
    }

    [Header("Main Objectives List")]
    [Tooltip("Primary objectives shown at the top")]
    public List<string> mainObjectives = new List<string>();

    [Header("Sub Objectives List")]
    [Tooltip("Secondary objectives shown at the bottom (like collection progress)")]
    public List<string> subObjectives = new List<string>();

    [Header("Inventory Reference")]
    public InventoryManager_Water inventoryManager;

    [Header("Item Tracker Reference")]
    public ObjectiveItemTracker_Water itemTracker;

    [Header("Intro Delay Settings")]
    [Tooltip("Total time (in seconds) before the first objective appears). Set to 0 to show immediately.")]
    public float introObjectiveDelay = 0f;
    [Tooltip("If true, objectives will show immediately regardless of delay")]
    public bool showObjectivesImmediately = true;

    private readonly HashSet<string> completedMainObjectives = new HashSet<string>();
    private readonly HashSet<string> completedSubObjectives = new HashSet<string>();
    private int currentMainObjectiveIndex = 0;

    // Store progress counters for objectives
    private Dictionary<string, string> objectiveProgressCounters = new Dictionary<string, string>();

    // Track which keys have been found
    private HashSet<string> foundKeys = new HashSet<string>();
    private bool breakerObjectiveComplete = false;

    private const string completeColorHex = "#00FF00FF"; // Green

    private void Start()
    {
        if (inventoryManager == null) inventoryManager = FindObjectOfType<InventoryManager_Water>();
        if (itemTracker == null) itemTracker = FindObjectOfType<ObjectiveItemTracker_Water>();

        // Setup text components
        if (mainObjectiveText != null)
        {
            mainObjectiveText.richText = true;
            mainObjectiveText.text = string.Empty;
        }

        if (subObjectiveText != null)
        {
            subObjectiveText.richText = true;
            subObjectiveText.text = string.Empty;
        }

        // Initialize main objectives - start with bedroom
        InitializeMainObjectives();

        // Add the persistent collection sub-objective
        if (!subObjectives.Contains("Collect All Items"))
        {
            subObjectives.Add("Collect All Items");
        }

        if (showObjectivesImmediately || introObjectiveDelay <= 0f)
        {
            UpdateUI();
        }
        else
        {
            StartCoroutine(DelayedObjectiveStart(introObjectiveDelay));
        }
    }

    private void InitializeMainObjectives()
    {
        mainObjectives.Clear();
        // Start with the first objective - bedroom for bag, flashlight, and basement key
        mainObjectives.Add("Go to the Bedroom to collect the Bag, Flashlight, and Basement Key");
    }

    private void Update()
    {
        // Auto-complete "Collect All Items" objective when all items are collected
        if (itemTracker != null && !IsSubObjectiveCompleted("Collect All Items"))
        {
            if (itemTracker.IsCompleted())
            {
                CompleteSubObjective("Collect All Items");
                Debug.Log("📦 Sub-objective auto-completed: Collect All Items");
            }
        }
    }

    // Method called by NarrationTrigger_Water to advance objectives
    public void TriggerNextObjectiveFromNarration()
    {
        if (currentMainObjectiveIndex < mainObjectives.Count)
        {
            string currentObjective = mainObjectives[currentMainObjectiveIndex];
            Debug.Log($"🎙️ Narration triggered - advancing from objective: {currentObjective}");
            UpdateUI();
        }
        else
        {
            Debug.Log("🎙️ Narration triggered but no current objective to advance");
        }
    }

    // Update progress counter for an objective
    public void UpdateObjectiveProgress(string objectiveName, int current, int total)
    {
        string counter = $"({current}/{total})";
        objectiveProgressCounters[objectiveName] = counter;
        Debug.Log($"🔄 Updated progress for '{objectiveName}': {counter}");
        UpdateUI();
    }

    // Called when a key is picked up
    public void OnKeyPickedUp(string keyName)
    {
        // Normalize the key name (remove "Key" suffix and spaces, convert to lowercase)
        string normalizedKey = keyName.Replace("Key", "").Replace(" ", "").ToLower().Trim();
        
        if (foundKeys.Contains(normalizedKey))
        {
            Debug.Log($"🗝️ Key '{normalizedKey}' already registered");
            return;
        }

        foundKeys.Add(normalizedKey);
        Debug.Log($"🗝️ Key registered: {normalizedKey} ({foundKeys.Count}/5) - Original name: '{keyName}'");

        // The ItemTracker handles objective completion and progression
        // This just tracks the count for potential future use
    }

    // Called when breaker is turned off
    public void OnBreakerTurnedOff()
    {
        if (breakerObjectiveComplete) return;
        breakerObjectiveComplete = true;

        Debug.Log("⚡ Breaker turned off - adding key finding objective!");

        // Insert ONLY the first key objective (Office Key)
        int insertIndex = currentMainObjectiveIndex;
        
        mainObjectives.Insert(insertIndex, "Find the Office Room Key in the Basement");

        Debug.Log($"📋 Inserted Office Key objective at index {insertIndex}");
        UpdateUI();
    }

    // Complete a main objective
    public void CompleteMainObjective(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("⚠️ Tried to complete an empty main objective!");
            return;
        }

        // Special handling for breaker objective
        if (name.Contains("Breaker") || name.Contains("power"))
        {
            OnBreakerTurnedOff();
        }

        // Find the objective in the list (partial match support)
        string matchedObjective = null;
        foreach (string obj in mainObjectives)
        {
            if (obj.Contains(name) || name.Contains(obj))
            {
                matchedObjective = obj;
                break;
            }
        }

        if (matchedObjective == null)
        {
            Debug.LogWarning($"⚠️ Main objective containing '{name}' not found in list!");
            return;
        }

        if (completedMainObjectives.Contains(matchedObjective))
        {
            Debug.Log($"ℹ️ Main objective '{matchedObjective}' is already completed.");
            return;
        }

        completedMainObjectives.Add(matchedObjective);
        Debug.Log($"✅ Main objective completed: {matchedObjective}");

        objectiveProgressCounters.Remove(matchedObjective);
        StartCoroutine(TemporaryCompleteFlash(matchedObjective, true));

        // Advance to next objective
        currentMainObjectiveIndex++;

        UpdateUI();
    }

    // Complete a sub objective
    public void CompleteSubObjective(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Debug.LogWarning("⚠️ Tried to complete an empty sub objective!");
            return;
        }

        if (completedSubObjectives.Contains(name))
        {
            Debug.Log($"ℹ️ Sub objective '{name}' is already completed.");
            return;
        }

        completedSubObjectives.Add(name);
        Debug.Log($"✅ Sub objective completed: {name}");

        objectiveProgressCounters.Remove(name);
        StartCoroutine(TemporaryCompleteFlash(name, false));

        UpdateUI();
    }

    // Legacy method for compatibility
    public void CompleteObjective(string name)
    {
        // Try to find in main objectives first (with partial matching)
        bool foundInMain = false;
        foreach (string obj in mainObjectives)
        {
            if (obj.Contains(name) || name.Contains(obj))
            {
                CompleteMainObjective(name);
                foundInMain = true;
                break;
            }
        }

        if (!foundInMain)
        {
            if (subObjectives.Contains(name))
            {
                CompleteSubObjective(name);
            }
            else
            {
                Debug.LogWarning($"⚠️ Objective '{name}' not found in main or sub objectives!");
            }
        }
    }

    // Add a new main objective dynamically (called by ItemTracker)
    public void AddMainObjective(string newObjective)
    {
        if (string.IsNullOrWhiteSpace(newObjective))
        {
            Debug.LogWarning("⚠️ Tried to add an empty main objective!");
            return;
        }

        if (mainObjectives.Contains(newObjective))
        {
            Debug.Log($"ℹ️ Main objective '{newObjective}' already exists.");
            return;
        }

        mainObjectives.Add(newObjective);
        Debug.Log($"🆕 New main objective added: {newObjective}");
        UpdateUI();
    }

    // Add a new sub objective dynamically
    public void AddSubObjective(string newObjective)
    {
        if (string.IsNullOrWhiteSpace(newObjective))
        {
            Debug.LogWarning("⚠️ Tried to add an empty sub objective!");
            return;
        }

        if (subObjectives.Contains(newObjective))
        {
            Debug.Log($"ℹ️ Sub objective '{newObjective}' already exists.");
            return;
        }

        subObjectives.Add(newObjective);
        Debug.Log($"🆕 New sub objective added: {newObjective}");
        UpdateUI();
    }

    private IEnumerator TemporaryCompleteFlash(string objectiveName, bool isMainObjective)
    {
        TextMeshProUGUI targetText = isMainObjective ? mainObjectiveText : subObjectiveText;
        if (targetText == null) yield break;

        string temp = $"<b>{objectiveName}</b> <color={completeColorHex}>(Complete)</color>\n\n";
        temp += targetText.text;
        targetText.text = temp;
        targetText.ForceMeshUpdate();

        yield return new WaitForSeconds(1.5f);
        UpdateUI();
    }

    public bool IsMainObjectiveCompleted(string name)
    {
        return completedMainObjectives.Contains(name);
    }

    public bool IsSubObjectiveCompleted(string name)
    {
        return completedSubObjectives.Contains(name);
    }

    public bool IsObjectiveCompleted(string name)
    {
        return IsMainObjectiveCompleted(name) || IsSubObjectiveCompleted(name);
    }

    private void UpdateUI()
    {
        if (mainObjectiveText == subObjectiveText && mainObjectiveText != null)
        {
            UpdateCombinedObjectiveUI();
        }
        else
        {
            UpdateMainObjectiveUI();
            UpdateSubObjectiveUI();
        }
    }

    private void UpdateCombinedObjectiveUI()
    {
        if (mainObjectiveText == null) return;

        string display = "";

        if (currentMainObjectiveIndex < mainObjectives.Count)
        {
            string objectiveName = mainObjectives[currentMainObjectiveIndex];
            bool isCompleted = completedMainObjectives.Contains(objectiveName);

            if (isCompleted)
            {
                display += $"{objectiveName} <color={completeColorHex}>(Complete)</color>\n";
            }
            else
            {
                if (objectiveProgressCounters.ContainsKey(objectiveName))
                {
                    display += $"{objectiveName} {objectiveProgressCounters[objectiveName]}\n";
                }
                else
                {
                    display += $"{objectiveName}\n";
                }
            }
        }

        if (subObjectives.Count > 0)
        {
            display += "\n";
        }

        foreach (string objectiveName in subObjectives)
        {
            bool isCompleted = completedSubObjectives.Contains(objectiveName);

            if (isCompleted)
            {
                display += $"{objectiveName} <color={completeColorHex}>(Complete)</color>\n";
            }
            else
            {
                if (objectiveProgressCounters.ContainsKey(objectiveName))
                {
                    display += $"{objectiveName} {objectiveProgressCounters[objectiveName]}\n";
                }
                else
                {
                    display += $"{objectiveName}\n";
                }
            }
        }

        mainObjectiveText.text = display.TrimEnd();
        mainObjectiveText.ForceMeshUpdate();
    }

    private void UpdateMainObjectiveUI()
    {
        if (mainObjectiveText == null) return;

        string display = "";

        if (currentMainObjectiveIndex < mainObjectives.Count)
        {
            string objectiveName = mainObjectives[currentMainObjectiveIndex];
            bool isCompleted = completedMainObjectives.Contains(objectiveName);

            if (isCompleted)
            {
                display = $"{objectiveName} <color={completeColorHex}>(Complete)</color>";
            }
            else
            {
                if (objectiveProgressCounters.ContainsKey(objectiveName))
                {
                    display = $"{objectiveName} {objectiveProgressCounters[objectiveName]}";
                }
                else
                {
                    display = objectiveName;
                }
            }
        }

        mainObjectiveText.text = display;
        mainObjectiveText.ForceMeshUpdate();
    }

    private void UpdateSubObjectiveUI()
    {
        if (subObjectiveText == null) return;

        string display = "";

        foreach (string objectiveName in subObjectives)
        {
            bool isCompleted = completedSubObjectives.Contains(objectiveName);

            if (isCompleted)
            {
                display += $"{objectiveName} <color={completeColorHex}>(Complete)</color>\n";
            }
            else
            {
                if (objectiveProgressCounters.ContainsKey(objectiveName))
                {
                    display += $"{objectiveName} {objectiveProgressCounters[objectiveName]}\n";
                }
                else
                {
                    display += $"{objectiveName}\n";
                }
            }
        }

        subObjectiveText.text = display.TrimEnd();
        subObjectiveText.ForceMeshUpdate();
    }

    private IEnumerator DelayedObjectiveStart(float delay)
    {
        Debug.Log($"⏳ Waiting {delay} seconds before showing first objective...");
        yield return new WaitForSeconds(delay);

        Debug.Log($"🎯 Showing first objective after delay");
        UpdateUI();
    }

    public void ShowObjectivesNow()
    {
        Debug.Log("🎯 ShowObjectivesNow() called — displaying objectives immediately.");
        StopAllCoroutines();
        UpdateUI();
    }

    [ContextMenu("Test Complete Current Main Objective")]
    public void TestCompleteCurrentMain()
    {
        if (currentMainObjectiveIndex < mainObjectives.Count)
            CompleteMainObjective(mainObjectives[currentMainObjectiveIndex]);
    }

    [ContextMenu("Debug: Show Current State")]
    private void DebugShowState()
    {
        Debug.Log("=== OBJECTIVE MANAGER STATE ===");
        Debug.Log($"Current Main Objective Index: {currentMainObjectiveIndex}");
        Debug.Log($"Total Main Objectives: {mainObjectives.Count}");
        Debug.Log($"Total Sub Objectives: {subObjectives.Count}");
        Debug.Log($"Completed Main: {completedMainObjectives.Count}");
        Debug.Log($"Completed Sub: {completedSubObjectives.Count}");
        Debug.Log($"Keys Found: {foundKeys.Count}/5");
        Debug.Log("==============================");
    }
}