using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HotDoorHandle : MonoBehaviour, IPickupable
{
    public static bool touchedHotHandle = false;
    public static bool DoorOpenedWithTowel { get; private set; } = false;

    [Header("References")]
    public Movements2 player;
    public SubtitleManager2 subtitleManager;
    public Image damageFlashImage;
    public Transform doorTransform;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool smartDoorOpen = true;
    [Tooltip("The door's forward direction (usually Vector3.right for doors hinged on left/right)")]
    public Vector3 doorForward = Vector3.right;

    [Header("Damage Settings")]
    public int burnDamage = 10;
    public float flashDuration = 0.3f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Audio")]
    public int burnSFX = 16;
    public int doorOpenSFX = 25;
    public int doorCloseSFX = 3;

    private bool doorLocked = true;
    private bool doorOpen = false;
    private bool playerInRange = false;
    private bool promptShown = false;

    private Quaternion closedRotation;

    void Start()
    {
        if (doorTransform != null)
            closedRotation = doorTransform.localRotation;

        // ✅ Restore door state from static flag
        if (DoorOpenedWithTowel)
        {
            doorLocked = false;
            Debug.Log("✅ HotDoorHandle restored: Door already opened with towel, now freely interactable");
        }

        Debug.Log($"HotDoorHandle.Start(): DoorOpenedWithTowel={DoorOpenedWithTowel}, touchedHotHandle={touchedHotHandle}, doorLocked={doorLocked}");
    }

    // ✅ Public method for save system to restore state
    public static void RestoreDoorState(bool openedWithTowel, bool touched)
    {
        DoorOpenedWithTowel = openedWithTowel;
        touchedHotHandle = touched;
        Debug.Log($"🚪 Restored door state: openedWithTowel={openedWithTowel}, touched={touched}");
    }

    // ✅ Reset on new game
    public static void ResetDoorProgress()
    {
        DoorOpenedWithTowel = false;
        touchedHotHandle = false;
        Debug.Log("🚪 Door progress reset");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            UpdatePrompt();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            GenericPickupButton.Instance.HidePickupPrompt();
            promptShown = false;
        }
    }

    void Update()
    {
        if (playerInRange && !promptShown)
        {
            UpdatePrompt();
        }
    }

    private void UpdatePrompt()
    {
        if (!SubtitleManager2.IntroStoryComplete) return;

        if (!EmergencyHotlineCall.IsHotlineActive && SubtitleManager2.CallObjectiveActive)
            return;

        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        if (doorLocked)
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Touch Door Handle");
            promptShown = true;
        }
        else
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Interact");
            promptShown = true;
        }
    }

    public void OnPickup()
    {
        if (!playerInRange) return;

        if (!SubtitleManager2.IntroStoryComplete)
        {
            Debug.Log("Cannot interact during intro story");
            return;
        }

        if (!EmergencyHotlineCall.IsHotlineActive && SubtitleManager2.CallObjectiveActive)
        {
            Debug.Log("Cannot interact with door until emergency call is complete");
            return;
        }

        // ✅ If door was already opened with towel, allow free interaction
        if (doorLocked && player.HasTowel())
        {
            player.UseTowel();
            subtitleManager.ShowCustomMessage("Good thing I have this cloth! The door is hot!", 3f, () =>
            {
                subtitleManager.ShowCustomMessage("I should close the door after me so the fire spreads slowly.", 4f, () =>
                {
                    DoorOpenedWithTowel = true;
                    Debug.Log("HotDoorHandle: Door opened with towel - FuseBox can now trigger");
                    subtitleManager.ShowObjective("Exit the bedroom");
                });
            });
            doorLocked = false;
            doorOpen = true;
            StartCoroutine(SwingDoor(true));
            GenericPickupButton.Instance.HidePickupPrompt();
            promptShown = false;
            Debug.Log("Door unlocked and opened using towel (no damage taken)");
            return;
        }

        if (doorLocked && !player.HasTowel())
        {
            player.TakeDamage(burnDamage);
            StartCoroutine(FlashDamage());
            subtitleManager.ShowCustomMessage("The handle is too hot!", 2.5f);
            subtitleManager.ShowObjective("Find something to protect your hand");

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(burnSFX);

            touchedHotHandle = true;
            Debug.Log("Touched hot handle - took damage!");
            return;
        }

        // ✅ Door is unlocked - allow free open/close
        if (!doorLocked)
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Interact");
            promptShown = true;

            if (doorOpen)
            {
                StartCoroutine(SwingDoor(false));
                doorOpen = false;
            }
            else
            {
                StartCoroutine(SwingDoor(true));
                doorOpen = true;
            }
        }
    }

    IEnumerator SwingDoor(bool opening)
    {
        if (doorTransform == null)
        {
            Debug.LogError("HotDoorHandle: No door transform assigned!");
            yield break;
        }

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider != null)
            playerCollider.enabled = false;

        if (AudioManager.Instance != null)
        {
            if (opening)
                AudioManager.Instance.PlaySFX(doorOpenSFX);
            else
                AudioManager.Instance.PlaySFX(doorCloseSFX);
        }

        float angle = openAngle;

        if (smartDoorOpen && player != null)
        {
            Vector3 doorToPlayer = (player.transform.position - doorTransform.position).normalized;
            Vector3 doorForwardWorld = doorTransform.TransformDirection(doorForward);
            float dot = Vector3.Dot(doorToPlayer, doorForwardWorld);

            if (dot > 0.1f)
                angle = -openAngle;
            else if (dot < -0.1f)
                angle = openAngle;
            else
                angle = openAngle;

            Debug.Log($"Smart Door: dot={dot:F2}, doorForward={doorForward}, angle={angle}°");
        }

        Quaternion startRotation = doorTransform.localRotation;
        Quaternion targetRotation = opening ? closedRotation * Quaternion.Euler(0, angle, 0) : closedRotation;

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * openSpeed;
            doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, progress);
            yield return null;
        }

        doorTransform.localRotation = targetRotation;

        if (playerCollider != null)
            playerCollider.enabled = true;

        Debug.Log(opening ? "Door opened away from player" : "Door closed");
    }

    IEnumerator FlashDamage()
    {
        if (damageFlashImage == null) yield break;
        damageFlashImage.enabled = true;
        damageFlashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        damageFlashImage.enabled = false;
    }
}