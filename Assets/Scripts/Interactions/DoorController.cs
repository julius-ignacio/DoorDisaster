<<<<<<< HEAD
=======
using System.Collections;
>>>>>>> 47c3962 (Quiz script changes)
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactionDistance = 4f;

<<<<<<< HEAD
    private bool isOpen = false;
    private bool isPlayerNear = false;
=======
    [Header("Item inside Cabinet")]
    public ItemPickup item; // assign the object inside

    private bool isOpen = false;
    private bool isAnimating = false;
>>>>>>> 47c3962 (Quiz script changes)
    private Transform playerTransform;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

<<<<<<< HEAD
=======
        if (item != null)
            item.SetInteractable(false);

>>>>>>> 47c3962 (Quiz script changes)
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
<<<<<<< HEAD
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNear = distanceToPlayer <= interactionDistance;

        if (isPlayerNear && !isOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
            }
        }
        else if (isPlayerNear && isOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleDoor();
            }
        }

        if (isOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, openSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, openSpeed * Time.deltaTime);
        }
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        Debug.Log(isOpen ? "Door Opened" : "Door Closed");
=======
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
>>>>>>> 47c3962 (Quiz script changes)
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 47c3962 (Quiz script changes)
