using UnityEngine;

public class HandCoverPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public Movements2 player;
    public GameObject clothObject;
    public SubtitleManager2 subtitleManager;

    private Outline outline;
    private bool hasPickedUp = false;
    private bool playerInRange = false;

    public static bool DoorObjectiveActive { get; set; } = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        // ✅ NEW: Check if cloth was already picked up (loaded from save)
        if (clothObject != null && !clothObject.activeInHierarchy)
        {
            hasPickedUp = true;
        }
    }

    void Update()
    {
        // ✅ CHANGED: Only show outline if backpack is picked up AND door objective is active
        if (DoorObjectiveActive && IsBackpackPickedUp() && !hasPickedUp)
        {
            if (outline != null)
                outline.enabled = true;
        }

        // Show button only if backpack is picked up
        if (playerInRange && !hasPickedUp && DoorObjectiveActive && IsBackpackPickedUp())
        {
            if (GameManager.Instance != null && !GameManager.Instance.isPaused)
            {
                if (GenericPickupButton.Instance != null &&
                    GenericPickupButton.Instance.pickupButton != null &&
                    !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Cloth");
                }
            }
        }
    }

    // ✅ NEW: Helper to check if backpack was picked up
    private bool IsBackpackPickedUp()
    {
        // Check inventory unlock
        if (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.IsBackpackUnlocked())
            return true;

        // Fallback: check if backpack GameObject is inactive
        var backpackObj = GameObject.Find("Backpack"); // Adjust name to match your backpack object
        if (backpackObj != null && !backpackObj.activeInHierarchy)
            return true;

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            // ✅ CHANGED: Only show if backpack is picked up
            if (DoorObjectiveActive && IsBackpackPickedUp())
            {
                if (GameManager.Instance == null || !GameManager.Instance.isPaused)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Cloth");
                }
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

    public void OnPickup()
    {
        if (!playerInRange || hasPickedUp) return;

        // ✅ CHANGED: Check backpack too
        if (!DoorObjectiveActive || !IsBackpackPickedUp())
        {
            Debug.Log("Cannot pick up cloth yet - pick up backpack first");
            return;
        }

        hasPickedUp = true;
        GenericPickupButton.Instance.HidePickupPrompt();

        if (outline != null)
            outline.enabled = false;

        if (clothObject != null)
            clothObject.SetActive(false);

        if (player != null)
            player.hasTowel = true;

        // ✅ NEW: Show appropriate message based on whether player touched door yet
        ShowClothPickupMessage();

        Debug.Log("✅ Cloth picked up!");
    }

    // ✅ NEW: Separated message logic for clarity
    private void ShowClothPickupMessage()
    {
        if (subtitleManager == null) return;

        if (HotDoorHandle.touchedHotHandle)
        {
            // Player already tried the door - remind them to use cloth
            subtitleManager.ShowCustomMessage(
                "Got the cloth! Now I can safely open the hot door.",
                2.5f,
                () => {
                    subtitleManager.ShowObjective("Use the cloth to open the bedroom door");
                }
            );
        }
        else
        {
            // Player hasn't touched door yet - guide them there
            subtitleManager.ShowCustomMessage(
                "This cloth might come in handy for the hot door.",
                2f,
                () => {
                    subtitleManager.ShowObjective("Go to the bedroom door");
                }
            );
        }
    }

    // ✅ NEW: Public method to check pickup status
    public bool HasBeenPickedUp()
    {
        return hasPickedUp;
    }
}