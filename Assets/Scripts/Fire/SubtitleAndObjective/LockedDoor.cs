using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LockedDoor : MonoBehaviour
{
    [Header("References")]
    public GameObject keyObject;
    public GameObject catObject;
    public Transform player;
    public GameObject timerUI;
    public SubtitleManager2 subtitleManager;
    public GameObject[] uiCanvases;
    public float rescueTime = 20f;
    public Transform safeHouseSpawn;
    public GameOverManager gameOverManager;

    private bool hasKey = false;
    private bool doorUnlocked = false;
    private bool timerRunning = false;
    private float currentTime;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isDoorOpen = false;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (timerUI != null)
            timerUI.SetActive(false);
    }

    void Update()
    {
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

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !doorUnlocked)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!hasKey)
                {
                    AudioManager.Instance.PlaySFX(32);

                    // Step 2: run sequence (pause → subs → timer)
                    StartCoroutine(ShowLockedSequence());
                }
                else
                {
                    UnlockDoor();
                }
            }
        }
    }

    private IEnumerator ShowLockedSequence()
    {
        // Step 1: hide HUD
        SetUICanvases(false);

        // Step 2: wait a moment so player hears SFX clearly
        yield return new WaitForSeconds(1.2f);

        // Step 3: show subtitle after pause
        ShowSubtitle("I need to hurry up and find the key!");
      

        // Step 4: show timer
        currentTime = rescueTime;
        if (timerUI != null) timerUI.SetActive(true);
        timerRunning = true;
    }

    private void UnlockDoor()
    {
        doorUnlocked = true;
        isDoorOpen = true;


        // Show HUD again
        SetUICanvases(true);

        if (timerUI != null) timerUI.SetActive(false);
        timerRunning = false;
        ShowSubtitle("The door is open! Save Mr. Kitty!");
    }

    public void OnKeyPickedUp()
    {
        hasKey = true;
        Debug.Log("LockedDoor: Key received!");
        ShowSubtitle("I found the key! Get back to the bedroom door.");
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
        // Hide the timer when player dies
        if (timerUI != null)
            timerUI.SetActive(false);

        timerRunning = false;

        // Show HUD again (optional, depends on your game over screen)
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
}