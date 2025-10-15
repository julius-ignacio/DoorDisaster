using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactionDistance = 4f;

    public GameObject doorOpenBtn;

    [Header("Item inside Cabinet")]
    public ItemPickup item; // assign the object inside

    private bool isOpen = false;
    private bool isAnimating = false;
    private Transform playerTransform;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        doorOpenBtn.SetActive(false);
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

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
            // Only toggle the door if player is not interacting with the item
            if (item == null || !item.HasBeenPickedUp())
            {
                StartCoroutine(AnimateDoor(!isOpen));
            }
        }
    }

    public void OpenDoor()
    {
            // Only toggle the door if player is not interacting with the item
            if (item == null || !item.HasBeenPickedUp())
            {
                StartCoroutine(AnimateDoor(!isOpen));
            }
    }

    private IEnumerator AnimateDoor(bool opening)
    {
        isAnimating = true;

        float elapsed = 0f;
        float duration = 1f / openSpeed;

        Quaternion startRot = transform.rotation;
        Quaternion endRot = opening ? openRotation : closedRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        transform.rotation = endRot;

        // Enable or disable the item only after fully open
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
