using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HotDoorHandle : MonoBehaviour, IPickupable
{
    public static bool touchedHotHandle = false;

    [Header("References")]
    public Movements2 player;
    public SubtitleManager2 subtitleManager;
    public Image damageFlashImage;
    public Transform doorTransform;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool smartDoorOpen = true;

    [Header("Damage Settings")]
    public int burnDamage = 10;
    public float flashDuration = 0.3f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f);

    [Header("Audio")]
    public int burnSFX = 16;           // Burn sound when touching hot handle
    public int doorOpenSFX = 25;       // Door opening sound
    public int doorCloseSFX = 3;      // Door closing sound

    private bool doorLocked = true;
    private bool doorOpen = false;
    private bool playerInRange = false;
    private bool promptShown = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        if (doorTransform != null)
            closedRotation = doorTransform.localRotation;
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
        // Keep prompt updated if player stays in trigger
        if (playerInRange && !promptShown)
        {
            UpdatePrompt();
        }
    }

    private void UpdatePrompt()
    {
        if (!SubtitleManager2.IntroStoryComplete) return;

        // ✅ Don't show prompt until emergency call is complete
        if (!EmergencyHotlineCall.IsHotlineActive && SubtitleManager2.CallObjectiveActive)
        {
            // Player needs to complete the call first
            return;
        }

        // Don't show if game is paused
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        // Before towel scene
        if (doorLocked)
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Touch Door Handle");
            promptShown = true;
        }
        else
        {
            // After towel scene, allow open/close anytime
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Interact");
            promptShown = true;
        }
    }

    public void OnPickup()
    {
        if (!playerInRange) return;

        // Prevent early interaction
        if (!SubtitleManager2.IntroStoryComplete)
        {
            Debug.Log("Cannot interact during intro story");
            return;
        }

        // ✅ Prevent interaction until emergency call is done
        if (!EmergencyHotlineCall.IsHotlineActive && SubtitleManager2.CallObjectiveActive)
        {
            Debug.Log("Cannot interact with door until emergency call is complete");
            return;
        }

        // ✅ SCENARIO 1: Player has towel BEFORE touching door (no damage!)
        if (doorLocked && player.HasTowel())
        {
            player.UseTowel();
            subtitleManager.ShowCustomMessage("Good thing I have this cloth! The door is hot!", 3f, () =>
            {
                subtitleManager.ShowCustomMessage("I should close the door after me so the fire spreads slowly.", 4f);
                subtitleManager.ShowObjective("Exit the bedroom");
            });
            doorLocked = false;
            doorOpen = true;
            StartCoroutine(SwingDoor(true));
            GenericPickupButton.Instance.HidePickupPrompt();
            promptShown = false;
            Debug.Log("Door unlocked and opened using towel (no damage taken)");
            return;
        }

        // ✅ SCENARIO 2: Player touches hot handle WITHOUT towel (takes damage!)
        if (doorLocked && !player.HasTowel())
        {
            player.TakeDamage(burnDamage);
            StartCoroutine(FlashDamage());
            subtitleManager.ShowCustomMessage("The handle is too hot!", 2.5f);
            subtitleManager.ShowObjective("Find something to protect your hand");

            // Play burn sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(burnSFX);

            touchedHotHandle = true; // ✅ This enables the towel pickup button
            Debug.Log("Touched hot handle - took damage!");
            return;
        }

        // ✅ Door unlocked — toggle open/close
        if (!doorLocked)
        {
            // 🔄 Immediately refresh the prompt (button reappears right away)
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

        // Play door sound at the start of animation
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
            Vector3 doorForward = doorTransform.right;
            float dot = Vector3.Dot(doorToPlayer, doorForward);
            angle = dot > 0 ? openAngle : -openAngle;
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
        Debug.Log(opening ? "Door opened" : "Door closed");
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