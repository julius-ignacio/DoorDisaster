using UnityEngine;

public class KeyPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public LockedDoor lockedDoor;           // Drag the door object here
    public GameObject keyVisual;             // The key 3D model
    public SubtitleManager2 subtitleManager;

    private bool playerInRange = false;
    private bool hasBeenPickedUp = false;
    public GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = true;

            // Don't show prompt if game is paused
            if (gameManager.isPaused)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Key");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GenericPickupButton.Instance.HidePickupPrompt();
        }
    }

    void Update()
    {
        if (playerInRange && !hasBeenPickedUp)
        {
            if (gameManager.isPaused)
            {
                // Check if prompt isn't already showing
                if (GenericPickupButton.Instance != null &&
                    GenericPickupButton.Instance.pickupButton != null &&
                    !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Key");
                }
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasBeenPickedUp) return;

        hasBeenPickedUp = true;

        // Tell the door we picked up the key
        if (lockedDoor != null)
        {
            lockedDoor.OnKeyPickedUp();
        }

        // Show message
        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "You found the key! Hurry back to the bedroom door!",
                4f
            );
        }

        // Hide the key visual
        if (keyVisual != null)
            keyVisual.SetActive(false);

        Debug.Log("Key picked up!");

        // Hide prompt before destroying
        GenericPickupButton.Instance.HidePickupPrompt();

        // Destroy this pickup trigger
        Destroy(gameObject);
    }
}