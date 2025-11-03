using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class WindowEscape : MonoBehaviour, IPickupable
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public GameObject heavyObject;
    public DoorFireTrigger doorFireTrigger;
    public Transform player;
    public HeavyObjectPickup heavyObjectPickup;

    [Header("UI to Hide After Escape")]
    public GameObject healthBar;
    public GameObject oxygenBar;
    public GameObject joystick;
    public GameObject jumpButton;

    [Header("Teleport Settings")]
    public Transform hallwaySpawnPoint;

    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 2f;

    [Header("Audio")]
    public int windowBreakSFX = 37;

    private bool hasHeavyObject = false;
    private bool hasEscaped = false;
    private bool promptShown = false;
    private bool playerInRange = false;

    void Start()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }

    void Update()
    {
        if (playerInRange && !hasEscaped)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isPaused)
            {
                if (GenericPickupButton.Instance != null &&
                    GenericPickupButton.Instance.pickupButton != null &&
                    !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
                {
                    if (doorFireTrigger != null && doorFireTrigger.HasShownFireMessage())
                    {
                        GenericPickupButton.Instance.ShowPickupPrompt(this, hasHeavyObject ? "Break Window" : "Try Window");
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEscaped)
        {
            if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
                return;

            playerInRange = true;

            if (GameManager.Instance == null || !GameManager.Instance.isPaused)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, hasHeavyObject ? "Break Window" : "Try Window");
            }

            if (!promptShown)
            {
                promptShown = true;
                subtitleManager.ShowCustomMessage(
                    "Let's try to open this window...",
                    2f,
                    () => subtitleManager.ShowObjective("Try the window")
                );
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
        if (!playerInRange || hasEscaped) return;

        if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
            return;

        if (!hasHeavyObject)
        {
            if (heavyObjectPickup != null)
            {
                heavyObjectPickup.EnablePickup();
            }

            subtitleManager.ShowCustomMessage(
                "Oh no, it's stuck! I need something heavy to break this open!",
                2f,
                () => subtitleManager.ShowObjective("Find a heavy object, try the lamp near the bed")
            );
        }
        else
        {
            StartEscape();
        }
    }

    public void PickupHeavyObject()
    {
        hasHeavyObject = true;

        if (heavyObject != null)
            heavyObject.SetActive(false);

        subtitleManager.HideObjective();
        subtitleManager.ShowCustomMessage(
            "Got it! This should break the window!",
            2f,
            () =>
            {
                subtitleManager.ShowObjective("Use the heavy object to break the bedroom window");

                if (playerInRange)
                {
                    GenericPickupButton.Instance.ShowPickupPrompt(this, "Break Window");
                }
            }
        );
    }

    private void StartEscape()
    {
        hasEscaped = true;
        subtitleManager.HideObjective();
        GenericPickupButton.Instance.HidePickupPrompt();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(windowBreakSFX);
            StartCoroutine(StopSoundAfterDelay(2f));
        }

        // Hide oxygen bar during escape subtitle
        if (oxygenBar != null)
            oxygenBar.SetActive(false);

        subtitleManager.ShowCustomMessage(
            "I broke the window! Time to get out!",
            2f,
            () => StartCoroutine(FadeTeleportSequence())
        );
    }

    private IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopAll();
    }

    private IEnumerator FadeTeleportSequence()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
        }

        if (player != null && hallwaySpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Movements2 movement = player.GetComponent<Movements2>();

            if (cc != null) cc.enabled = false;

            player.position = hallwaySpawnPoint.position;
            player.rotation = Quaternion.Euler(0f, 270f, 0f);

            if (cc != null) cc.enabled = true;
        }

        if (healthBar != null)
            healthBar.SetActive(false);

        if (subtitleManager != null)
            subtitleManager.HideObjective();

        yield return new WaitForSeconds(0.2f);

        PlayerOxygen.InHallwayChase = true;

        // ✅ Refill oxygen to max BEFORE showing the bar
        PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
        if (oxygen != null)
        {
            oxygen.RefillOxygen(); // Reset to maximum
        }

        if (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.backpackButton != null)
        {
            InventoryManager_fire.Instance.backpackButton.SetActive(false);
        }

        if (fadeOverlay != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
            fadeOverlay.gameObject.SetActive(false);
        }

        // ✅ Show subtitle FIRST, then show oxygen bar AFTER subtitle completes
        subtitleManager.ShowCustomMessage(
            "Where am I? I need to find the exit before I run out of air!",
            3f,
            () =>
            {
                subtitleManager.ShowObjective("Find the exit door - hurry!");

                // ✅ Show oxygen bar AFTER subtitle ends
                if (oxygen != null)
                {
                    oxygen.ShowOxygenBar();
                }
            }
        );

        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveTrialData(DataManager.Instance.currentTrial);
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        float elapsed = 0f;
        Color c = fadeOverlay.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadeOverlay.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        fadeOverlay.color = new Color(c.r, c.g, c.b, endAlpha);
    }

    public bool HasHeavyObject()
    {
        return hasHeavyObject;
    }
}