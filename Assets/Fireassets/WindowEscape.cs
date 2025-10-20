using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class WindowEscape : MonoBehaviour, IPickupable
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public GameObject heavyObject;
    public DoorFireTrigger doorFireTrigger;
    public Transform player;

    [Header("Teleport Settings")]
    public Transform hallwaySpawnPoint;

    [Header("Fade Settings")]
    public Image fadeOverlay;
    public float fadeDuration = 1f;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasEscaped)
        {
            // ✅ Wait until the SDR message (doorFireTrigger) has been shown
            if (doorFireTrigger != null && !doorFireTrigger.HasShownFireMessage())
                return;

            playerInRange = true;

            // ✅ Always show the button when player is near
            GenericPickupButton.Instance.ShowPickupPrompt(this, hasHeavyObject ? "Break Window" : "Try Window");

            // ✅ Only show subtitle/objective once (the first time)
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

        // 🚪 Window is locked, need heavy object
        if (!hasHeavyObject)
        {
            subtitleManager.ShowCustomMessage(
                "Oh no, it's stuck! I need something heavy to break this open!",
                2f,
                () => subtitleManager.ShowObjective("Find a heavy object - try the lamp near the bed")
            );
        }
        else
        {
            StartEscape();
        }
    }

    // ✅ Called by HeavyObjectPickup after the player picks up the object
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

                // ✅ Show the button again if player is near the window
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

        subtitleManager.ShowCustomMessage(
            "I broke the window! Time to get out!",
            2f,
            () => StartCoroutine(FadeTeleportSequence())
        );
    }

    private IEnumerator FadeTeleportSequence()
    {
        // 1️⃣ Fade to black
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // 2️⃣ Teleport player
        if (player != null && hallwaySpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Movements2 movement = player.GetComponent<Movements2>();

            if (cc != null) cc.enabled = false;

            player.position = hallwaySpawnPoint.position;
            player.rotation = Quaternion.Euler(0f, 270f, 0f);

            if (cc != null) cc.enabled = true;
        }

        yield return new WaitForSeconds(0.2f);

        // 3️⃣ Fade back in
        if (fadeOverlay != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
            fadeOverlay.gameObject.SetActive(false);
        }

        // 4️⃣ Show next objective
        subtitleManager.ShowCustomMessage(
            "I made it out! Now I need to find the exit.",
            3f,
            () => subtitleManager.ShowObjective("Find the exit door")
        );

        // 5️⃣ Save progress
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveStageData(DataManager.Instance.currentTrial, DataManager.Instance.currentStage);
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
