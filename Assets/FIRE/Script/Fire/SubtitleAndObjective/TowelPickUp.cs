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
    public Image fadeOverlay; // ✅ Use the Image directly (BlackTP)

    [Header("Fade Settings")]
    public float fadeDuration = 3f;

    [Header("Cat Audio")]
    public AudioSource catAudio;

    [Header("Outline Settings")]
    private Outline outline;

    private bool hasPickedUp = false;
    private bool playerInRange = false;

    // ✅ Static flag to track teleport state (persists across scene/saves)
    public static bool HasTeleportedToHouseB { get; private set; } = false;

    // ✅ Reset static flag on new game/restart
    public static void ResetTeleportProgress()
    {
        HasTeleportedToHouseB = false;
        Debug.Log("🏠 Towel teleport progress reset");
    }

    void Awake()
    {
        // Stop cat audio at start
        if (catAudio != null)
            catAudio.Stop();
    }

    void Start()
    {
        // Get outline component
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        // ✅ Setup fade overlay (BlackTP)
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.enabled = true;

            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
        else
        {
            Debug.LogWarning("Fade overlay (BlackTP) not assigned in TowelPickup!");
        }

        // ✅ If already teleported, disable this pickup completely
        if (HasTeleportedToHouseB)
        {
            DisablePickup();
        }
    }

    // ✅ For loading from save data
    public static void RestoreTeleportState(bool teleported)
    {
        HasTeleportedToHouseB = teleported;
        Debug.Log($"🏠 Restored teleport state: {teleported}");
    }

    void Update()
    {
        // ✅ Show outline only after breaker puzzle is complete AND not picked up yet
        if (outline != null && !hasPickedUp && !HasTeleportedToHouseB)
        {
            outline.enabled = BreakerPuzzle.BreakerPuzzleComplete;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ✅ Don't allow interaction if already picked up or teleported
        if (other.CompareTag("Player") && !hasPickedUp && !HasTeleportedToHouseB)
        {
            playerInRange = true;

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
        if (!playerInRange || hasPickedUp || HasTeleportedToHouseB) return;

        if (!BreakerPuzzle.BreakerPuzzleComplete)
        {
            Debug.Log("Cannot pick up towel before breaker puzzle is complete");
            return;
        }

        hasPickedUp = true;

        // ✅ Hide prompt immediately
        GenericPickupButton.Instance.HidePickupPrompt();
        playerInRange = false; // ✅ Force range to false

        towel.SetActive(false);

        if (outline != null)
            outline.enabled = false;

        subtitleManager.HideObjective();

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
        if (fadeOverlay == null)
        {
            Debug.LogError("Fade overlay Image not found!");
            yield break;
        }

        // ✅ Fade to black
        yield return StartCoroutine(FadeImage(fadeOverlay, 0f, 1f));

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

            HasTeleportedToHouseB = true; // ✅ Set flag after successful teleport
            Debug.Log("✅ Player teleported to House B - flag set");

            // ✅ Disable this pickup system completely after teleport
            DisablePickup();
        }
        else
        {
            Debug.LogError("Teleport failed: Player or House B Spawn Point not assigned!");
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        // ✅ Fade back in
        yield return StartCoroutine(FadeImage(fadeOverlay, 1f, 0f));

        // Play cat audio AFTER teleport
        if (catAudio != null)
            catAudio.Play();

        // Dialogue sequence
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

    // ✅ Smooth fade helper for Image
    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha)
    {
        if (img == null) yield break;

        float elapsed = 0f;
        Color c = img.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            img.color = c;
            yield return null;
        }

        c.a = endAlpha;
        img.color = c;
    }

    // ✅ Completely disable the pickup after use
    private void DisablePickup()
    {
        playerInRange = false;
        hasPickedUp = true;

        GenericPickupButton.Instance.HidePickupPrompt();

        if (outline != null)
            outline.enabled = false;

        if (towel != null)
            towel.SetActive(false);

        // Disable the collider so no more triggers fire
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Debug.Log("🧼 Towel pickup disabled after teleport");
    }

    public bool HasPickedUpTowel()
    {
        return hasPickedUp;
    }
}