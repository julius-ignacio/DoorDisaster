using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenCloseDoor1 : MonoBehaviour, IInteractable_Water
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
        return isOpen ? "Press E: Close Door" : "Press E: Open Door";
    }

    public void Interact()
    {
        ToggleDoor();
    }
    // =========================

    private void ToggleDoor()
    {
        if (animator == null)
        {
            Debug.LogError($"{name}: No Animator found/assigned!");
            return;
        }

        if (!isOpen)
        {
            animator.Play("Opening 1"); // Match your animation state name
            isOpen = true;
        }
        else
        {
            animator.Play("Closing 1"); // Match your animation state name
            isOpen = false;
        }
    }
}
