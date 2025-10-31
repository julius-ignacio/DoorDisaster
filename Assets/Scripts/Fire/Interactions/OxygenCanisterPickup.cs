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

    // ✅ Check if player is still in range when story completes
    void Update()
    {
        // ✅ Don't show button if game is paused
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        // If player is in range but prompt isn't showing yet, check if story is now complete
        if (playerInRange && !hasPickedUp && SubtitleManager2.IntroStoryComplete)
        {
            // Show the prompt if it wasn't shown before (because story just completed)
            if (GenericPickupButton.Instance != null && GenericPickupButton.Instance.pickupButton != null)
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
        if (!playerInRange || hasPickedUp) return;

        hasPickedUp = true;
        canister.SetActive(false);
        GenericPickupButton.Instance.HidePickupPrompt();

        PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
        if (oxygen != null)
        {
            oxygen.RefillOxygen();
            Debug.Log("Oxygen canister picked up - oxygen refilled!");
        }
    }
}