using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloodFactTrigger : MonoBehaviour
{
    [Header("Flood Fact Settings")]
    [Tooltip("The Flood Fact data to show when the player enters the trigger.")]
    public FloodFact fact;           // ScriptableObject with the fact content

    [Tooltip("The UI prefab that displays the Flood Fact.")]
    public FloodFactUI factUIPrefab; // UI prefab reference

    [Tooltip("If true, the fact will only appear once per game.")]
    public bool showOnce = true;

    private bool factShown = false;

    private void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
        else
            Debug.LogWarning($"{name}: Missing Collider component!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!factShown && other.CompareTag("Player"))
        {
            if (factUIPrefab == null)
            {
                Debug.LogError($"{name}: No FloodFactUI prefab assigned!");
                return;
            }

            if (fact == null)
            {
                Debug.LogError($"{name}: No FloodFact data assigned!");
                return;
            }

            // ✅ Skip if already discovered (from DataManager)
            if (showOnce && DataManager_Water.Instance != null)
            {
                string factId = fact.name;
                if (DataManager_Water.Instance.HasItem($"fact:{factId}"))
                {
                    Debug.Log($"[FloodFactTrigger] Fact '{factId}' already discovered earlier.");
                    return;
                }
            }

            // Find Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"{name}: No Canvas found in the scene!");
                return;
            }

            // Remove old fact UI if needed
            FloodFactUI existingUI = canvas.GetComponentInChildren<FloodFactUI>();
            if (existingUI != null)
                Destroy(existingUI.gameObject);

            // Spawn and show new fact UI
            FloodFactUI ui = Instantiate(factUIPrefab, canvas.transform);
            ui.SetFact(fact);

            // ✅ Register in DataManager
            if (DataManager_Water.Instance != null)
            {
                DataManager_Water.Instance.AddFact(fact.name);
            }

            factShown = true;
            Debug.Log($"✅ Discovered new fact: {fact.name}");
        }
    }
}
