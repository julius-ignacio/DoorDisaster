using UnityEngine;

public class KeyPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public LockedDoor lockedDoor;           // Drag the door object here
    public GameObject keyVisual;             // The key 3D model
    public SubtitleManager2 subtitleManager;

    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Key");
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

    public void OnPickup()
    {
        if (!playerInRange) return;

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

        // Destroy this pickup trigger
        Destroy(gameObject);
    }
}