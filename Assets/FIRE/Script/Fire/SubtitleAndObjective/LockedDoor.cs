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
    public GameObject pauseButton;
    public float rescueTime = 60f;
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
    private bool hasTriedDoor = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isDoorOpen = false;

    // ✅ Static flags for save system
    public static bool HasKey { get; private set; } = false;
    public static bool DoorUnlocked { get; private set; } = false;
    public static bool HasTriedDoor { get; private set; } = false;
    public static bool TimerWasRunning { get; private set; } = false;
    public static float SavedTime { get; private set; } = 0f;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (timerUI != null)
            timerUI.SetActive(false);

        // ✅ Restore state from previous session
        RestoreState();
    }

    // ✅ Restore door state
    public void RestoreState()
    {
        hasKey = HasKey;
        doorUnlocked = DoorUnlocked;
        hasTriedDoor = HasTriedDoor;
        timerRunning = TimerWasRunning;
        currentTime = SavedTime;

        if (doorUnlocked)
        {
            isDoorOpen = true;
            transform.rotation = openRotation;
            if (timerUI != null) timerUI.SetActive(false);
            if (pauseButton != null) pauseButton.SetActive(true);
        }
        else if (timerRunning && currentTime > 0)
        {
            if (timerUI != null) timerUI.SetActive(true);
            if (pauseButton != null) pauseButton.SetActive(false);
            Debug.Log($"🔑 Resuming timer with {currentTime} seconds remaining");
        }

        Debug.Log($"🔑 LockedDoor restored: HasKey={hasKey}, Unlocked={doorUnlocked}, Tried={hasTriedDoor}, Timer={timerRunning}");
    }


    // ✅ Public method for save system
    public static void RestoreDoorState(bool key, bool unlocked, bool tried, bool timer, float time)
    {
        HasKey = key;
        DoorUnlocked = unlocked;
        HasTriedDoor = tried;
        TimerWasRunning = timer;
        SavedTime = time;
        Debug.Log($"🔑 Restored door state: key={key}, unlocked={unlocked}, tried={tried}, timer={timer}, time={time}");
    }

    // ✅ Reset on new game
    public static void ResetDoorProgress()
    {
        HasKey = false;
        DoorUnlocked = false;
        HasTriedDoor = false;
        TimerWasRunning = false;
        SavedTime = 0f;
        Debug.Log("🔑 Door progress reset");
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
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
            SavedTime = currentTime; // ✅ Save time continuously

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
                TimerWasRunning = false; // ✅ Update static flag
                FailRescue();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !doorUnlocked)
        {
            playerInRange = true;

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
            if (!hasTriedDoor)
            {
                hasTriedDoor = true;
                HasTriedDoor = true; // ✅ Update static flag
                AudioManager.Instance.PlaySFX(32);
                GenericPickupButton.Instance.HidePickupPrompt();
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
        // ✅ Hide pause button
        if (pauseButton != null)
            pauseButton.SetActive(false);

        // Hide HUD
        SetUICanvases(false);

        yield return new WaitForSeconds(1.2f);

        ShowSubtitle("I need to hurry up and find the key!");

        // Show timer
        currentTime = rescueTime;
        SavedTime = currentTime; // ✅ Save initial time
        if (timerUI != null) timerUI.SetActive(true);
        timerRunning = true;
        TimerWasRunning = true; // ✅ Update static flag

        // Update objective
        if (subtitleManager != null)
            subtitleManager.ShowObjective("Find the key before time runs out!");
    }

    private void UnlockDoor()
    {
        doorUnlocked = true;
        DoorUnlocked = true; // ✅ Update static flag
        isDoorOpen = true;
        timerRunning = false;
        TimerWasRunning = false; // ✅ Update static flag

        PlaySound(doorOpenSoundIndex);

        // ✅ Show pause button again
        if (pauseButton != null)
            pauseButton.SetActive(true);

        SetUICanvases(true);

        // ✅ Ensure oxygen drain resumes
        if (player != null)
        {
            PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
            if (oxygen != null)
            {
                oxygen.EnsureOxygenDrainActive();
                Debug.Log("🔓 Door unlocked - oxygen drain resumed");
            }
        }

        if (timerUI != null) timerUI.SetActive(false);
        GenericPickupButton.Instance.HidePickupPrompt();
        ShowSubtitle("The door is open! Save Mr. Kitty!");

        // Update objective
        if (subtitleManager != null)
            subtitleManager.ShowObjective("Rescue Mr. Kitty!");
    }

    public void OnKeyPickedUp()
    {
        hasKey = true;
        HasKey = true; // ✅ Update static flag
        Debug.Log("LockedDoor: Key received!");
        ShowSubtitle("I found the key! Get back to the bedroom door.");

        // Update objective
        if (subtitleManager != null)
            subtitleManager.ShowObjective("Return to the bedroom door and unlock it");

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

        // ✅ Show pause button again
        if (pauseButton != null)
            pauseButton.SetActive(true);

        timerRunning = false;
        SetUICanvases(true);

        // ✅ Ensure oxygen drain resumes even on failure
        if (player != null)
        {
            PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
            if (oxygen != null)
            {
                oxygen.EnsureOxygenDrainActive();
                Debug.Log("⏱️ Timer failed - oxygen drain resumed");
            }
        }

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