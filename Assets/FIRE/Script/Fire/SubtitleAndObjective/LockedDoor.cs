using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LockedDoor : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject keyObject;
    public GameObject catObject;
    public Transform player;
    public GameObject timerUI;
    public SubtitleManager2 subtitleManager;
    public GameObject[] uiCanvases;
    public float rescueTime = 60f; // Changed to 60 seconds (1 minute)
    public Transform safeHouseSpawn;
    public GameOverManager gameOverManager;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    [Header("Audio")]
    public int doorOpenSoundIndex = -1;

    private bool hasKey = false;
    private bool doorUnlocked = false;
    private bool timerRunning = false;
    private float currentTime;
    private bool playerInRange = false;
    private bool hasTriedDoor = false; // Track if player already tried the door

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isDoorOpen = false;
    public GameManager gameManager;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (timerUI != null)
            timerUI.SetActive(false);
    }

    void Update()
    {
        // ✅ Don't update anything if game is paused
        if (gameManager.isPaused)
            return;

        // Smooth door animation
        if (isDoorOpen)
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, openSpeed * Time.deltaTime);
        else
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, openSpeed * Time.deltaTime);

        // Countdown logic
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            if (timerUI != null)
            {
                TextMeshProUGUI tmpText = timerUI.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpText != null)
                    tmpText.text = "Time Left: " + Mathf.Ceil(currentTime);
                else
                {
                    Text uiText = timerUI.GetComponentInChildren<Text>();
                    if (uiText != null)
                        uiText.text = "Time Left: " + Mathf.Ceil(currentTime);
                }
            }

            if (currentTime <= 0)
            {
                timerRunning = false;
                FailRescue();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorUnlocked)
        {
            playerInRange = true;

            // Only show prompt if player has key OR hasn't tried the door yet
            if (hasKey)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Unlock Door");
            }
            else if (!hasTriedDoor)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Try Door");
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
        if (!playerInRange || doorUnlocked) return;

        if (!hasKey)
        {
            // Only trigger if player hasn't tried the door yet
            if (!hasTriedDoor)
            {
                hasTriedDoor = true; // Mark as tried
                AudioManager.Instance.PlaySFX(32);
                GenericPickupButton.Instance.HidePickupPrompt(); // Hide button immediately
                StartCoroutine(ShowLockedSequence());
            }
        }
        else
        {
            UnlockDoor();
        }
    }

    private IEnumerator ShowLockedSequence()
    {
        // Hide HUD
        SetUICanvases(false);

        // Wait for SFX to play
        yield return new WaitForSeconds(1.2f);

        // Show subtitle
        ShowSubtitle("I need to hurry up and find the key!");

        // Show timer
        currentTime = rescueTime;
        if (timerUI != null) timerUI.SetActive(true);
        timerRunning = true;
    }

    private void UnlockDoor()
    {
        doorUnlocked = true;
        isDoorOpen = true;

        // Play door opening sound
        PlaySound(doorOpenSoundIndex);

        // Show HUD again
        SetUICanvases(true);

        if (timerUI != null) timerUI.SetActive(false);
        timerRunning = false;
        GenericPickupButton.Instance.HidePickupPrompt();
        ShowSubtitle("The door is open! Save Mr. Kitty!");
    }

    public void OnKeyPickedUp()
    {
        hasKey = true;
        Debug.Log("LockedDoor: Key received!");
        ShowSubtitle("I found the key! Get back to the bedroom door.");

        // Update button text if player is still in range
        if (playerInRange && GenericPickupButton.Instance != null)
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Unlock Door");
        }
    }

    public void PickUpCat()
    {
        if (doorUnlocked)
        {
            if (catObject != null) Destroy(catObject);
            StartCoroutine(RescueCat());
        }
    }

    private IEnumerator RescueCat()
    {
        ShowSubtitle("I've got you, Mr. Kitty!");
        yield return new WaitForSeconds(2f);

        if (player != null && safeHouseSpawn != null)
            player.position = safeHouseSpawn.position;

        ShowSubtitle("We're safe now...");
    }

    private void FailRescue()
    {
        if (timerUI != null)
            timerUI.SetActive(false);

        timerRunning = false;
        SetUICanvases(true);

        GameOverManager.TriggerDeath(
            "TIME RAN OUT",
            "The ceiling collapsed! You failed to rescue Mr. Kitty in time..."
        );
    }

    private void ShowSubtitle(string message)
    {
        if (subtitleManager != null)
            subtitleManager.ShowCustomMessage(message, 3f);
        else
            Debug.Log("Subtitle: " + message);
    }

    private void SetUICanvases(bool state)
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas != null && canvas != timerUI)
                canvas.SetActive(state);
        }
    }

    private void PlaySound(int soundIndex)
    {
        if (soundIndex >= 0 && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(soundIndex);
    }
}