using UnityEngine;

public class PhonePickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public EmergencyHotlineCall hotlineCall;
    public GameObject phoneModel; // Optional: hide phone after pickup

    private bool playerInRange = false;
    private bool hasPickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;
            // Only show prompt if intro story is complete and game is not paused
            if (SubtitleManager2.IntroStoryComplete && !GameManager.Instance.isPaused)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Phone");
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

    // Check if story just completed while player is in range
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (playerInRange && !hasPickedUp && SubtitleManager2.IntroStoryComplete)
        {
            if (GenericPickupButton.Instance != null && GenericPickupButton.Instance.pickupButton != null)
            {
                // Only show if it's not already showing
                if (!GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Phone");
                }
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasPickedUp) return;

        hasPickedUp = true;
        GenericPickupButton.Instance.HidePickupPrompt();

        // Optional: hide the phone model
        if (phoneModel != null)
            phoneModel.SetActive(false);

        // Trigger the hotline call system
        if (hotlineCall != null)
        {
            hotlineCall.TriggerHotlineObjective();
        }
        else
        {
            Debug.LogError("PhonePickup: EmergencyHotlineCall reference not assigned!");
        }
    }
}