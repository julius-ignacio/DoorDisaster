using UnityEngine;

public class KeyPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public LockedDoor lockedDoor;
    public GameObject keyVisual;
    public SubtitleManager2 subtitleManager;

    private bool playerInRange = false;
    private bool hasBeenPickedUp = false;

    // ✅ Static flag for save system
    public static bool KeyPickedUp { get; private set; } = false;

    void Start()
    {
        // ✅ If key was already picked up, hide it
        if (KeyPickedUp)
        {
            if (keyVisual != null)
                keyVisual.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = true;
            if (GameManager.Instance == null || !GameManager.Instance.isPaused)
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
            if (GameManager.Instance != null && !GameManager.Instance.isPaused)
            {
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
        KeyPickedUp = true; // ✅ Set static flag

        if (lockedDoor != null)
        {
            lockedDoor.OnKeyPickedUp();
        }

        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "You found the key! Hurry back to the bedroom door!",
                4f
            );
        }

        if (keyVisual != null)
            keyVisual.SetActive(false);

        Debug.Log("Key picked up!");
        GenericPickupButton.Instance.HidePickupPrompt();
        Destroy(gameObject);
    }

    // ✅ For save system
    public static void RestoreKeyState(bool pickedUp)
    {
        KeyPickedUp = pickedUp;
        Debug.Log($"🔑 Restored key state: picked={pickedUp}");
    }

    public static void ResetKeyProgress()
    {
        KeyPickedUp = false;
        Debug.Log("🔑 Key progress reset");
    }
}