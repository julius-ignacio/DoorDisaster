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

    // ✅ Flag to track when door objective is active
    public static bool DoorObjectiveActive { get; set; } = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false; // Start hidden
    }

    void Update()
    {
        // ✅ Show outline only AFTER touching hot door (visual hint)
        if (HotDoorHandle.touchedHotHandle && !hasPickedUp)
        {
            if (outline != null)
                outline.enabled = true;
        }

        // ✅ Show button only if door objective is active
        if (playerInRange && !hasPickedUp && DoorObjectiveActive)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isPaused)
            {
                // Check if prompt isn't already showing
                if (GenericPickupButton.Instance != null &&
                    GenericPickupButton.Instance.pickupButton != null &&
                    !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Cloth");
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            // ✅ Only show button if door objective is active
            if (DoorObjectiveActive)
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

        // ✅ Can only pick up if door objective is active
        if (!DoorObjectiveActive)
        {
            Debug.Log("Cannot pick up cloth yet - complete previous objectives first");
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

        // ✅ Show message based on if they touched door first
        if (subtitleManager != null)
        {
            if (HotDoorHandle.touchedHotHandle)
            {
                // Picked up AFTER touching hot door
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
                // Picked up BEFORE touching door (smart player!)
                subtitleManager.ShowCustomMessage(
                    "This cloth might come in handy for the hot door.",
                    2f,
                    () => {
                        subtitleManager.ShowObjective("Go to the bedroom door");
                    }
                );
            }
        }

        Debug.Log("Cloth picked up!");
    }
}