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
    private bool canPickup = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        // ✅ NEW: Check if backpack was already picked up (loaded from save)
        if (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.IsBackpackUnlocked())
        {
            hasBeenPickedUp = true;
            if (backpackModel != null)
                backpackModel.SetActive(false);
            gameObject.SetActive(false);
            return;
        }

        // ✅ NEW: Check if hotline is already done on load
        CheckHotlineStatus();
    }

    void Update()
    {
        // ✅ Keep checking if 911 call completes during gameplay
        if (!canPickup)
        {
            CheckHotlineStatus();
        }
    }

    // ✅ NEW: Robust method to check hotline completion
    private void CheckHotlineStatus()
    {
        if (canPickup) return; // Already enabled

        // Method 1: Check static flag (most reliable)
        EmergencyHotlineCall hotline = FindObjectOfType<EmergencyHotlineCall>();
        if (hotline != null && hotline.HasCalledHotline())
        {
            EnablePickup();
            return;
        }

        // Method 2: Check if phone model is inactive (fallback)
        PhonePickup phone = FindObjectOfType<PhonePickup>();
        if (phone != null && phone.phoneModel != null && !phone.phoneModel.activeInHierarchy)
        {
            EnablePickup();
            return;
        }

        // Method 3: Check if intro is complete AND CallObjectiveActive is false (means hotline was done)
        if (SubtitleManager2.IntroStoryComplete && !SubtitleManager2.CallObjectiveActive)
        {
            EnablePickup();
            return;
        }
    }

    private void EnablePickup()
    {
        if (canPickup) return;

        canPickup = true;
        Debug.Log("✅ Backpack pickup enabled (hotline completed)");

        // Show outline when ready to pickup
        if (outline != null)
            outline.enabled = true;

        // If player is already in range, show prompt immediately
        if (playerInRange && !hasBeenPickedUp)
        {
            GenericPickupButton.Instance.ShowPickupPrompt(this, "Pick Up Backpack");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = true;

            // Only show pickup prompt after 911 call
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
        Debug.Log("✅ Backpack picked up!");

        if (outline != null)
            outline.enabled = false;

        GenericPickupButton.Instance.HidePickupPrompt();

        // Unlock the inventory UI
        if (InventoryManager_fire.Instance != null)
        {
            InventoryManager_fire.Instance.UnlockBackpack();
            Debug.Log("✅ Inventory UI unlocked!");
        }

        // Show message and then door objective
        if (subtitleManager != null)
        {
            subtitleManager.ShowCustomMessage(
                "Got my backpack! Now I need to get out of here safely.",
                2f,
                () =>
                {
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