using UnityEngine;

public class PhonePickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public EmergencyHotlineCall hotlineCall;
    public GameObject phoneModel; // Keep this visible!

    private bool playerInRange = false;
    private bool hasPickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            if (SubtitleManager2.IntroStoryComplete &&
                !GameManager.Instance.isPaused &&
                GenericPickupButton.Instance != null)
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
            GenericPickupButton.Instance?.HidePickupPrompt();
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (playerInRange && !hasPickedUp && SubtitleManager2.IntroStoryComplete)
        {
            if (GenericPickupButton.Instance != null &&
                GenericPickupButton.Instance.pickupButton != null &&
                !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Phone");
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasPickedUp) return;

        hasPickedUp = true;
        Debug.Log("✅ Phone picked up (phone stays visible)");

        GenericPickupButton.Instance?.HidePickupPrompt();

        // ✅ Phone model stays visible - don't hide it!
        // Players should still be able to see the phone after calling

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