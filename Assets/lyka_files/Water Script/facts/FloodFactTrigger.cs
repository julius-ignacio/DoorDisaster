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

            // ✅ If showOnce is true, stop after showing once
            if (showOnce && factShown)
                return;

            // Find a Canvas in the scene
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError($"{name}: No Canvas found in the scene!");
                return;
            }

            // Remove any existing FloodFactUI on screen
            FloodFactUI existingUI = canvas.GetComponentInChildren<FloodFactUI>();
            if (existingUI != null)
                Destroy(existingUI.gameObject);

            // ✅ Create and display new Flood Fact UI
            FloodFactUI ui = Instantiate(factUIPrefab, canvas.transform);
            ui.SetFact(fact);

            factShown = true;
            Debug.Log($"✅ Discovered new fact: {fact.name}");
        }
    }
}
