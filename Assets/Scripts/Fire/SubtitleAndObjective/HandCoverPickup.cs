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
    }

    void Update()
    {
        // ✅ Show outline when door objective is active (instead of after touching door)
        if (DoorObjectiveActive && !hasPickedUp)
        {
            if (outline != null)
                outline.enabled = true;
        }

        // Show button only if door objective is active
        if (playerInRange && !hasPickedUp && DoorObjectiveActive)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

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

        if (subtitleManager != null)
        {
            if (HotDoorHandle.touchedHotHandle)
            {
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