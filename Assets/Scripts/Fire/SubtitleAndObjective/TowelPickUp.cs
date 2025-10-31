using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowelPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject towel;
    public SubtitleManager2 subtitleManager;
    public Transform player;
    public Transform houseBSpawnPoint;
    public Image fadeOverlay;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    [Header("Cat Audio")]
    public AudioSource catAudio;

    [Header("Outline Settings")]
    private Outline outline;

    void Awake()
    {
        // Make sure cat audio doesn't play at start
        if (catAudio != null)
            catAudio.Stop();
    }

    private bool hasPickedUp = false;
    private bool playerInRange = false;

    void Start()
    {
        // Get outline component
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false; // Hidden at start

        // ✅ FIX: Set fade overlay to fully transparent at start
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true); // Keep active
            CanvasGroup cg = fadeOverlay.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = fadeOverlay.gameObject.AddComponent<CanvasGroup>();
            }
            cg.alpha = 0f; // Start transparent
            cg.blocksRaycasts = false;
        }
    }

    void Update()
    {
        // ✅ Show outline only after breaker puzzle is complete
        if (outline != null && !hasPickedUp)
        {
            outline.enabled = BreakerPuzzle.BreakerPuzzleComplete;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;

            // ✅ Only show prompt if breaker puzzle is complete
            if (BreakerPuzzle.BreakerPuzzleComplete)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Towel");
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

        // ✅ Extra safety check
        if (!BreakerPuzzle.BreakerPuzzleComplete)
        {
            Debug.Log("Cannot pick up towel before breaker puzzle is complete");
            return;
        }

        hasPickedUp = true;
        towel.SetActive(false);

        // Disable outline
        if (outline != null)
            outline.enabled = false;

        subtitleManager.HideObjective();
        GenericPickupButton.Instance.HidePickupPrompt();

        PlayerOxygen oxygen = player.GetComponent<PlayerOxygen>();
        if (oxygen != null)
            oxygen.EquipTowel();

        subtitleManager.ShowCustomMessage(
            "Got the wet towel! This will help me breathe.",
            2f,
            () => StartCoroutine(FadeTeleportSequence())
        );
    }

    private IEnumerator FadeTeleportSequence()
    {
        // ✅ FIX: Use CanvasGroup for smooth fading
        CanvasGroup fadeGroup = null;
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeGroup = fadeOverlay.GetComponent<CanvasGroup>();
            if (fadeGroup == null)
            {
                fadeGroup = fadeOverlay.gameObject.AddComponent<CanvasGroup>();
            }
            fadeGroup.blocksRaycasts = true; // Block input during fade

            yield return StartCoroutine(FadeCanvasGroup(fadeGroup, 0f, 1f));
        }

        // Teleport player
        if (player != null && houseBSpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (cc != null) cc.enabled = false;
            player.position = houseBSpawnPoint.position;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogError("Teleport failed: Player or House B Spawn Point not assigned!");
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        // Fade back in
        if (fadeOverlay != null && fadeGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(fadeGroup, 1f, 0f));
            fadeGroup.blocksRaycasts = false;
        }

        // Play cat audio AFTER player is teleported to House B
        if (catAudio != null)
            catAudio.Play();

        subtitleManager.ShowCustomMessage(
            "Huh..? What just happened? This doesn't look right...",
            3f,
            () =>
            {
                subtitleManager.ShowCustomMessage(
                    "Huh? Mr. Kitty? I need to save him!",
                    3f,
                    () =>
                    {
                        subtitleManager.ShowCustomMessage(
                            "But the fire is blocking my way! I should find a way to put it out.",
                            3f,
                            () =>
                            {
                                // Stop cat audio after subtitles finish
                                if (catAudio != null)
                                    catAudio.Stop();

                                subtitleManager.ShowObjective("Find the fire extinguisher");
                            }
                        );
                    }
                );
            }
        );
    }

    // ✅ NEW: Fade using CanvasGroup (more reliable)
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        if (canvasGroup == null) yield break;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    public bool HasPickedUpTowel()
    {
        return hasPickedUp;
    }
}