using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    [Tooltip("Set this to 0 for first checkpoint, 1 for second, etc.")]
    public int checkpointIndex;

    [Header("Visual (Optional)")]
    [Tooltip("The yellow glow effect - will be disabled after triggering")]
    public GameObject visualEffect;

    [Header("Audio (Optional)")]
    public AudioSource checkpointSound;

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Activate checkpoint
            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.SetCheckpoint(checkpointIndex);
            }

            // Optional: Play sound
            if (checkpointSound != null)
            {
                checkpointSound.Play();
            }

            // Optional: Disable visual after triggered
            if (visualEffect != null)
            {
                visualEffect.SetActive(false);
            }

            Debug.Log($"✅ Player reached checkpoint {checkpointIndex}");
        }
    }

    // Optional: Show gizmo in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}