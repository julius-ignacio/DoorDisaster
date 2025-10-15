using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Settings")]
    public bool isEssential = false;
    public string itemName = "Item";

    [Header("References")]
    public SubtitleManager2 subtitleManager;
    private ObjectiveManager objectiveManager;

    [Header("Outline Settings (URP)")]
    public Material outlineMaterial;   // Assign your URP Outline material in Inspector
    private Material originalMaterial; // To restore original
    private Renderer rend;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private bool hasBeenPickedUp = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
        }

        objectiveManager = FindObjectOfType<ObjectiveManager>();

        DisableOutline(); // start without glow
    }

    void Update()
    {
        if (objectiveManager == null || hasBeenPickedUp) return;

        int stage = objectiveManager.GetObjectiveStage();

        // ✅ Only glow during collecting essentials
        if (stage >= 2 && isEssential)
        {
            EnableOutline();
        }
        else
        {
            DisableOutline();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                int stage = (objectiveManager != null) ? objectiveManager.GetObjectiveStage() : 0;

                if (stage >= 2)
                {
                    if (isEssential)
                        PickupItem();
                    else
                        ShowNonEssentialMessage();
                }
                else
                {
                    subtitleManager?.ShowCustomMessage("I should focus on packing first...", 1.5f, null);
                }
            }
        }
    }

    private void PickupItem()
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;

        subtitleManager?.ShowCustomMessage(
            isEssential ? $"Picked up {itemName}. This could be important!" : $"Picked up {itemName}.",
            1.5f, null
        );

        DisableOutline(); // remove glow when picked

        // Fade out then hide
        StartCoroutine(FadeOutAndHide());

        objectiveManager?.OnItemPickedUp(itemName, isEssential);
    }

    private void ShowNonEssentialMessage()
    {
        subtitleManager?.ShowCustomMessage(
            $"You don't need that... {itemName} can stay behind.",
            2f, null
        );
    }

    private void EnableOutline()
    {
        if (rend != null && outlineMaterial != null && rend.material != outlineMaterial)
        {
            rend.material = outlineMaterial;
        }
    }

    private void DisableOutline()
    {
        if (rend != null && originalMaterial != null && rend.material != originalMaterial)
        {
            rend.material = originalMaterial;
        }
    }

    private IEnumerator FadeOutAndHide()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null) yield break;

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

        // Hide object
        gameObject.SetActive(false);
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public bool HasBeenPickedUp()
    {
        return hasBeenPickedUp;
    }

    // ✅ New method to enable/disable interaction (for cabinet)
    public void SetInteractable(bool canInteract)
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = canInteract;

        // Optional: remove outline when disabled
        if (!canInteract)
            DisableOutline();
    }
}
