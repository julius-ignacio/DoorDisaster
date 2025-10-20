using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenCloseCabinet : MonoBehaviour, IInteractable_Water
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Setup")]
    [SerializeField] private bool ensureNonTriggerCollider = true;

    private bool isOpen = false;

    private void Reset()
    {
        // Auto-assign animator if present
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (ensureNonTriggerCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col != null && col.isTrigger)
            {
                col.isTrigger = false;
                Debug.LogWarning($"{name}: Collider was Trigger. Set to non-Trigger for raycast interaction.");
            }
        }
    }

    // ===== IInteractable =====
    public string GetPrompt()
    {
        return isOpen ? "Press E: Close Cabinet" : "Press E: Open Cabinet";
    }

    public void Interact()
    {
        ToggleCabinet();
    }
    // =========================

    public void ToggleCabinet()
    {
        if (animator == null)
        {
            Debug.LogError($"{name}: No Animator found/assigned!");
            return;
        }

        if (!isOpen)
        {
            animator.Play("Opening"); // <-- Match your animation name!
            isOpen = true;
        }
        else
        {
            animator.Play("Closing"); // <-- Match your animation name!
            isOpen = false;
        }
    }
}
