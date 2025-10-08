using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("References")]
    public LockedDoor lockedDoor;           // Drag the door object here
    public GameObject keyVisual;             // The key 3D model
    public SubtitleManager subtitleManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
}