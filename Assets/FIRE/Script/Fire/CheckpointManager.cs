using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Header("Checkpoint Transforms")]
    [Tooltip("Drag your checkpoint GameObjects here in order (0, 1, 2, etc.)")]
    public Transform[] checkpoints;

    private int currentCheckpointIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called when player enters a checkpoint trigger
    public void SetCheckpoint(int index)
    {
        if (index >= 0 && index < checkpoints.Length)
        {
            currentCheckpointIndex = index;
            Debug.Log($"✅ Checkpoint {index} activated");

            // Auto-save when checkpoint reached
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SaveGameState();
            }
        }
    }

    // Get current checkpoint index for saving
    public int GetCurrentCheckpointIndex()
    {
        return currentCheckpointIndex;
    }

    // Load checkpoint and teleport player
    public void LoadCheckpoint(int index)
    {
        if (index >= 0 && index < checkpoints.Length && checkpoints[index] != null)
        {
            currentCheckpointIndex = index;

            // Find player and teleport
            var player = FindObjectOfType<Movements2>(true);
            if (player != null)
            {
                var controller = player.GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.enabled = false;
                    player.transform.position = checkpoints[index].position;
                    player.transform.rotation = checkpoints[index].rotation;
                    controller.enabled = true;
                }
                else
                {
                    player.transform.position = checkpoints[index].position;
                    player.transform.rotation = checkpoints[index].rotation;
                }

                Debug.Log($"✅ Player teleported to checkpoint {index}");
            }
        }
        else
        {
            Debug.LogWarning($"Checkpoint {index} not found or invalid");
        }
    }
}