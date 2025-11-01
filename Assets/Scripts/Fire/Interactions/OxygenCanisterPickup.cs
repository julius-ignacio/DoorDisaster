using UnityEngine;

public class OxygenCanisterPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject canister;
    public Transform player;

    private bool hasPickedUp = false;
    private bool playerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            // ✅ Only show prompt if intro story is complete and game is not paused
            if (SubtitleManager2.IntroStoryComplete && !GameManager.Instance.isPaused)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Oxygen Canister");
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
        // ✅ Don't show button if game is paused
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        // If player is in range but prompt isn't showing yet, check if story is now complete
        if (playerInRange && !hasPickedUp && SubtitleManager2.IntroStoryComplete)
        {
            if (GenericPickupButton.Instance != null &&
                GenericPickupButton.Instance.pickupButton != null)
            {
                // Only show if it's not already showing
                if (!GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Oxygen Canister");
                }
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasPickedUp)
            return;

        hasPickedUp = true;

        // ✅ Add to inventory instead of using immediately
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddOxygenCanister();
            canister.SetActive(false);
            GenericPickupButton.Instance.HidePickupPrompt();
            Debug.Log("Oxygen canister added to inventory!");
        }
        else
        {
            Debug.LogError("InventoryManager not found!");
        }
    }
}