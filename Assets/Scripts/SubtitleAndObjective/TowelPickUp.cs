using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowelPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject towel;
    public SubtitleManager subtitleManager;
    public Transform player;
    public Transform houseBSpawnPoint;

    [Header("Fade Settings")]
    public Image fadeOverlay;      // Assign the same fade image used for death/wake-up
    public float fadeDuration = 1f;

    private bool hasPickedUp = false;

    void Start()
    {
        // Ensure fade overlay starts invisible
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true); // keep it active
            Color c = fadeOverlay.color;
            c.a = 0f; // invisible at start
            fadeOverlay.color = c;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            if (Input.GetKey(KeyCode.E))
            {
                hasPickedUp = true;
                towel.SetActive(false);
                subtitleManager.HideObjective();

                PlayerOxygen oxygen = other.GetComponent<PlayerOxygen>();
                if (oxygen != null)
                    oxygen.EquipTowel();

                // Start fade + teleport sequence after first line
                subtitleManager.ShowCustomMessage(
                    "Got the wet towel! This will help me breathe.",
                    2f,
                    () => StartCoroutine(FadeTeleportSequence())
                );
            }
        }
    }

    private IEnumerator FadeTeleportSequence()
    {
        // 1️⃣ Fade out
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
        }

        // 2️⃣ Teleport player
        if (player != null && houseBSpawnPoint != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (cc != null) cc.enabled = false;

            player.position = houseBSpawnPoint.position;

            if (rb != null)
            {
<<<<<<< HEAD
                rb.velocity = Vector3.zero;
=======
                rb.linearVelocity = Vector3.zero;
>>>>>>> 47c3962 (Quiz script changes)
                rb.angularVelocity = Vector3.zero;
            }

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogError("Teleport failed: Player or House B Spawn Point not assigned!");
            yield break;
        }

        yield return new WaitForSeconds(0.2f); // optional delay for smoothness

        // 3️⃣ Fade in
        if (fadeOverlay != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
            fadeOverlay.gameObject.SetActive(false); // hide after fading in
        }

        // 4️⃣ Continue subtitle sequence
        subtitleManager.ShowCustomMessage(
            "Huh..? What just happened? This doesn't look right...",
            3f,
            () =>
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null) audioSource.Play();

                // Mr. Kitty line
                subtitleManager.ShowCustomMessage(
                    "Huh? Mr. Kitty? I need to save him!",
                    3f,
                    () =>
                    {
                        // Fire blocking line
                        subtitleManager.ShowCustomMessage(
                            "But the fire is blocking my way! I should find a way to put it out.",
                            3f,
                            () =>
                            {
                                // Update objective
                                subtitleManager.ShowObjective("Find the fire extinguisher");
                            }
                        );
                    }
                );
            }
        );
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color color = fadeOverlay.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeOverlay.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeOverlay.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    public bool HasPickedUpTowel()
    {
        return hasPickedUp;
    }
}
