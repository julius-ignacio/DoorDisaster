using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IPickupable
{
    [Header("Item Settings")]
    public bool isEssential = false;
    public string itemName = "Item";

    [Header("References")]
    public SubtitleManager2 subtitleManager;
    private ObjectiveManager objectiveManager;
    public CabinetOpener cabinetOpener;

    [Header("Outline Settings")]
    private Outline outline;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    private bool hasBeenPickedUp = false;
    private bool playerInRange = false;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        objectiveManager = FindObjectOfType<ObjectiveManager>();
    }

    void Update()
    {
        if (objectiveManager == null || hasBeenPickedUp || outline == null) return;

        int stage = objectiveManager.GetObjectiveStage();

        if (isEssential && stage >= 1)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = true;
            int stage = (objectiveManager != null) ? objectiveManager.GetObjectiveStage() : 0;

            if (cabinetOpener != null && !cabinetOpener.IsCabinetOpen())
                return;

            if (isEssential && stage >= 1)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, $"Pick Up {itemName}");
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
        if (other.CompareTag("Player") && !hasBeenPickedUp && playerInRange && cabinetOpener != null && cabinetOpener.IsCabinetOpen())
        {
            int stage = (objectiveManager != null) ? objectiveManager.GetObjectiveStage() : 0;

            if (isEssential && stage >= 1)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, $"Pick Up {itemName}");
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || hasBeenPickedUp) return;

        int stage = (objectiveManager != null) ? objectiveManager.GetObjectiveStage() : 0;

        if (isEssential && stage >= 1)
        {
            PickupItem();
        }
        else
        {
            subtitleManager?.ShowCustomMessage("I should focus on packing first...", 1.5f, null);
        }
    }

    private void PickupItem()
    {
        if (hasBeenPickedUp) return;

        hasBeenPickedUp = true;

        subtitleManager?.ShowCustomMessage(
            $"Picked up {itemName}. This could be important!",
            1.5f,
            null
        );

        if (outline != null)
            outline.enabled = false;

        StartCoroutine(FadeOutAndHide());

        objectiveManager?.OnItemPickedUp(itemName, isEssential);

        if (itemName.Contains("Oxygen") || itemName.Contains("oxygen"))
        {
            InventoryManager.Instance?.AddOxygenCanister();
        }

        if (isEssential)
        {
            InventoryManager.Instance?.AddEssentialItem();
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

        gameObject.SetActive(false);
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    public bool HasBeenPickedUp()
    {
        return hasBeenPickedUp;
    }

    public void SetInteractable(bool canInteract)
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = canInteract;

        if (!canInteract && outline != null)
            outline.enabled = false;
    }
}
