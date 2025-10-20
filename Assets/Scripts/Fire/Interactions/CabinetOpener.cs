using System.Collections;
using UnityEngine;

public class CabinetOpener : MonoBehaviour, IPickupable
{
    public enum CabinetType { SlideForward, SwingDoor }

    [Header("Cabinet Settings")]
    public CabinetType cabinetType = CabinetType.SlideForward; // choose in Inspector
    public Transform cabinetDoor;
    public float openDistance = 0.5f;
    public float openSpeed = 2f;
    public float openAngle = 90f; // for SwingDoor type

    [Header("Item inside the cabinet")]
    public ItemPickup item;

    [Header("References")]
    public SubtitleManager2 subtitleManager;
    public ObjectiveManager objectiveManager;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Vector3 closedPosition;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool playerInRange = false;

    void Start()
    {
        if (cabinetDoor != null)
        {
            closedPosition = cabinetDoor.localPosition;
            closedRotation = cabinetDoor.localRotation;
            openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        }

        if (item != null)
            item.SetInteractable(false);
    }

    public bool IsCabinetOpen() => isOpen;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isAnimating && !isOpen)
        {
            playerInRange = true;
            if (objectiveManager != null && objectiveManager.IsBackpackPickedUp())
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Open Cabinet");
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
        if (other.CompareTag("Player") && !isAnimating && playerInRange && !isOpen)
        {
            if (objectiveManager != null && objectiveManager.IsBackpackPickedUp())
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Open Cabinet");
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || isAnimating || isOpen) return;

        if (objectiveManager != null && !objectiveManager.IsBackpackPickedUp())
        {
            subtitleManager?.ShowCustomMessage("I should focus on packing first.", 1.5f, null);
            return;
        }

        if (item != null && item.HasBeenPickedUp())
            return;

        StartCoroutine(AnimateCabinetOpen());
    }

    private IEnumerator AnimateCabinetOpen()
    {
        isAnimating = true;

        subtitleManager?.ShowCustomMessage("I found a cabinet.", 1.5f, null);

        float elapsedTime = 0f;
        float duration = 1f / openSpeed;

        Vector3 startPos = cabinetDoor.localPosition;
        Quaternion startRot = cabinetDoor.localRotation;

        Vector3 endPos = startPos;
        Quaternion endRot = startRot;

        if (cabinetType == CabinetType.SlideForward)
        {
            endPos = closedPosition + Vector3.forward * openDistance;
            endRot = closedRotation;
        }
        else if (cabinetType == CabinetType.SwingDoor)
        {
            endPos = closedPosition;
            endRot = openRotation;
        }

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            cabinetDoor.localPosition = Vector3.Lerp(startPos, endPos, t);
            cabinetDoor.localRotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        cabinetDoor.localPosition = endPos;
        cabinetDoor.localRotation = endRot;

        // ✅ Enable the item only after door fully opens
        if (item != null)
            item.SetInteractable(true);

        // ✅ Hide prompt permanently after use
        GenericPickupButton.Instance.HidePickupPrompt();

        // ✅ Refresh pickup prompt for item if player still nearby
        if (playerInRange && item != null)
        {
            yield return new WaitForSeconds(0.1f);
            GenericPickupButton.Instance.ShowPickupPrompt(item, $"Pick Up {item.itemName}");
        }

        isOpen = true;
        isAnimating = false;
    }
}
