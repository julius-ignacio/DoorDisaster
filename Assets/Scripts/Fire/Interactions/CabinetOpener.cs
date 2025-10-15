using System.Collections;
using UnityEngine;

public class CabinetOpener : MonoBehaviour
{
    [Header("Cabinet Settings")]
    public Transform cabinetDoor;
    public float openDistance = 0.5f; // How far to pull the door
    public float openSpeed = 2f;      // How fast to open
    public Quaternion openRotation = Quaternion.Euler(0, 0, 0);

    [Header("Item inside the cabinet")]
    public ItemPickup item; // Reference your ItemPickup script

    [Header("References")]
    public SubtitleManager2 subtitleManager;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Vector3 closedPosition;
    private Quaternion closedRotation;

    void Start()
    {
        if (cabinetDoor != null)
        {
            closedPosition = cabinetDoor.localPosition;
            closedRotation = cabinetDoor.localRotation;
        }

        if (item != null)
            item.SetInteractable(false); // Disable item at start
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isAnimating)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
  
            }
        }
    }


    public void openCabinet(){         
        
                if (item != null && item.HasBeenPickedUp())
                    return;

                StartCoroutine(AnimateCabinet(!isOpen));}

    private IEnumerator AnimateCabinet(bool opening)
    {
        isAnimating = true;

        if (opening && subtitleManager != null)
            subtitleManager.ShowCustomMessage("I found a cabinet.", 1.5f, null);

        float elapsedTime = 0f;
        float duration = 1f / openSpeed;

        Vector3 startPos = cabinetDoor.localPosition;
        Vector3 endPos = opening ? closedPosition + Vector3.forward * openDistance : closedPosition;
        Quaternion startRot = cabinetDoor.localRotation;
        Quaternion endRot = opening ? openRotation : closedRotation;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            cabinetDoor.localPosition = Vector3.Lerp(startPos, endPos, t);
            cabinetDoor.localRotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        // Snap to final position
        cabinetDoor.localPosition = endPos;
        cabinetDoor.localRotation = endRot;

        // Enable or disable item after animation
        if (item != null)
            item.SetInteractable(opening);

        isOpen = opening;
        isAnimating = false;
    }
}
