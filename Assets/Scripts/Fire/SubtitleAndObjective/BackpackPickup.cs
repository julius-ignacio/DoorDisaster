using UnityEngine;
using System.Collections;

public class BackpackPickup : MonoBehaviour, IPickupable
{
    [Header("References")]
    public GameObject backpackModel; // The visual backpack in the world
    public SubtitleManager2 subtitleManager;

    [Header("Outline Settings")]
    private Outline outline;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private bool hasBeenPickedUp = false;
    private bool playerInRange = false;
    private bool canPickup = false; // Only allow pickup after 911 call

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    void Update()
    {
        // ✅ Check if 911 call is complete to enable pickup
        if (!canPickup)
        {
            // Check if hotline call is done
            EmergencyHotlineCall hotline = FindObjectOfType<EmergencyHotlineCall>();
            if (hotline != null && hotline.HasCalledHotline())
            {
                canPickup = true;

                // Show outline when ready to pickup
                if (outline != null)
                    outline.enabled = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = true;

            // ✅ Only show pickup prompt after 911 call
            if (canPickup)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Backpack");
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

    void OnTriggerStay(Collider other)
    {
        // Show prompt if player enters range after 911 call completes
        if (other.CompareTag("Player") && !hasBeenPickedUp && playerInRange && canPickup)
        {
            if (GenericPickupButton.Instance != null &&
                GenericPickupButton.Instance.pickupButton != null &&
                !GenericPickupButton.Instance.pickupButton.gameObject.activeSelf)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Backpack");
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasBeenPickedUp || !canPickup) return;

        PickupBackpack();
    }

    private void PickupBackpack()
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;

        if (outline != null)
            outline.enabled = false;

        GenericPickupButton.Instance.HidePickupPrompt();

        // ✅ Unlock the inventory UI
        if (InventoryManager_fire.Instance != null)
        {
            InventoryManager_fire.Instance.UnlockBackpack();
            Debug.Log("Backpack picked up - inventory UI unlocked!");
        }

        // ✅ Show message and then door objective
        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Got my backpack! Now I need to get out of here safely.",
                2f,
                () =>
                {
                    // ✅ Show door objective after backpack message
                    subtitleManager.ShowObjective("Exit the bedroom - find a way to open the door safely");
                }
            );
        }

        // Fade out and hide the backpack model
        StartCoroutine(FadeOutAndHide());
    }

    private IEnumerator FadeOutAndHide()
    {
        if (backpackModel == null)
        {
            gameObject.SetActive(false);
            yield break;
        }

        Renderer renderer = backpackModel.GetComponent<Renderer>();
        if (renderer == null)
        {
            backpackModel.SetActive(false);
            yield break;
        }

        Color startColor = renderer.material.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            Color newColor = startColor;
            newColor.a = alpha;
            renderer.material.color = newColor;
            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 0f;
        renderer.material.color = finalColor;

        backpackModel.SetActive(false);
    }

    public bool HasBeenPickedUp()
    {
        return hasBeenPickedUp;
    }
}