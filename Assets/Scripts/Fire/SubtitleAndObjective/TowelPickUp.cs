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
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            playerInRange = true;
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Towel");
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

        hasPickedUp = true;
        towel.SetActive(false);
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
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f));
        }

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

        if (fadeOverlay != null)
        {
            yield return StartCoroutine(Fade(1f, 0f));
            fadeOverlay.gameObject.SetActive(false);
        }

        // Play cat audio AFTER player is teleported to House B
        if (catAudio != null)
            catAudio.Play();

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