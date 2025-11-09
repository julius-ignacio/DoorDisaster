using System.Collections;
using UnityEngine;

public class CabinetOpener : MonoBehaviour, IPickupable
{
    public enum CabinetType { SlideForward, SwingDoor }

    [Header("Cabinet Settings")]
    public CabinetType cabinetType = CabinetType.SlideForward;
    public Transform cabinetDoor;
    public float openDistance = 0.5f;
    public float openSpeed = 2f;
    public float openAngle = 90f;

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
        if (other.CompareTag("Player") && !isAnimating)
        {
            playerInRange = true;

            // ✅ Only show cabinet prompt if cabinet is still closed and at collecting stage
            if (!isOpen && objectiveManager != null && objectiveManager.GetObjectiveStage() >= 1)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Open Cabinet");
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
        // ✅ Only show cabinet prompt if not open and not animating
        if (other.CompareTag("Player") && !isAnimating && playerInRange && !isOpen)
        {
            if (objectiveManager != null && objectiveManager.GetObjectiveStage() >= 1)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(this, "Open Cabinet");
            }
        }
    }

    public void OnPickup()
    {
        if (!playerInRange || isAnimating || isOpen) return;

        // ✅ Check if we're at the collecting stage
        if (objectiveManager != null && objectiveManager.GetObjectiveStage() < 1)
        {
            subtitleManager?.ShowCustomMessage("I should focus on escaping first.", 1.5f, null);
            return;
        }

        if (item != null && item.HasBeenPickedUp())
            return;

        StartCoroutine(AnimateCabinetOpen());
    }

    private IEnumerator AnimateCabinetOpen()
    {
        isAnimating = true;

        // ✅ Hide cabinet prompt immediately
        GenericPickupButton.Instance.HidePickupPrompt();

        subtitleManager?.ShowCustomMessage("Let me check what's inside...", 1.5f, null);

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

        isOpen = true;

        // ✅ Enable the item after door opens
        if (item != null)
            item.SetInteractable(true);

        isAnimating = false;

        // ✅ Wait a moment, then show item prompt if player still in range
        yield return new WaitForSeconds(0.3f);

        if (playerInRange && item != null && !item.HasBeenPickedUp())
        {
            // ✅ Check if we're at collecting stage to show item prompt
            int stage = (objectiveManager != null) ? objectiveManager.GetObjectiveStage() : 0;

            if (item.isEssential && stage >= 1)
            {
                GenericPickupButton.Instance.ShowPickupPrompt(item, $"Pick Up {item.itemName}");
            }
        }
    }
}