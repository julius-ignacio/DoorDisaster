using System.Collections;
using UnityEngine;

public class DoubleDoorController : MonoBehaviour
{
    [Header("Doors")]
    public Transform leftDoor;
    public Transform rightDoor;
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactionDistance = 4f;

    [Header("Item inside Cabinet")]
    public ItemPickup item; // Reference to your ItemPickup

    [Header("Optional")]
    public SubtitleManager subtitleManager;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Transform playerTransform;

    private Quaternion leftClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightClosedRotation;
    private Quaternion rightOpenRotation;

    void Start()
    {
        if (leftDoor != null)
        {
            leftClosedRotation = leftDoor.localRotation;
            leftOpenRotation = leftClosedRotation * Quaternion.Euler(0, openAngle, 0);
        }

        if (rightDoor != null)
        {
            rightClosedRotation = rightDoor.localRotation;
            rightOpenRotation = rightClosedRotation * Quaternion.Euler(0, -openAngle, 0);
        }

        if (item != null)
            item.SetInteractable(false);

        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (isAnimating) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        bool playerNearby = distanceToPlayer <= interactionDistance;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Only toggle if player is not interacting with item
            if (item == null || !item.HasBeenPickedUp())
            {
                StartCoroutine(AnimateDoors(!isOpen));
            }
        }
    }

    private IEnumerator AnimateDoors(bool opening)
    {
        isAnimating = true;

        if (opening && subtitleManager != null)
            subtitleManager.ShowCustomMessage("I found a cabinet.", 1.5f, null);

        float elapsed = 0f;
        float duration = 1f / openSpeed;

        Quaternion leftStart = leftDoor.localRotation;
        Quaternion leftEnd = opening ? leftOpenRotation : leftClosedRotation;

        Quaternion rightStart = rightDoor.localRotation;
        Quaternion rightEnd = opening ? rightOpenRotation : rightClosedRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            leftDoor.localRotation = Quaternion.Lerp(leftStart, leftEnd, t);
            rightDoor.localRotation = Quaternion.Lerp(rightStart, rightEnd, t);

            yield return null;
        }

        leftDoor.localRotation = leftEnd;
        rightDoor.localRotation = rightEnd;

        // Enable or disable the item after animation
        if (item != null)
            item.SetInteractable(opening);

        isOpen = opening;
        isAnimating = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
