using UnityEngine;

public class FrontDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float interactionDistance = 3f;

    private bool isOpen = false;
    private bool isPlayerNear = false;
    private Transform playerTransform;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        // Store the original rotation as "closed"
        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        // Find the player
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        // Check if player is near the door
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNear = distanceToPlayer <= interactionDistance;

        // Show interaction prompt (you can replace this with UI later)
        if (isPlayerNear && !isOpen)
        {
            // Display "Press E to Open" - for now just in console
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

        // Smoothly rotate the door
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
    }

    // Draw interaction range in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}