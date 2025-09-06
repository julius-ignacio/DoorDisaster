using UnityEngine;
using UnityEngine.UI;

public class PickupScript : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 5f;
    public KeyCode pickupKey = KeyCode.E;
    public Transform holdPos; // Position where objects are held

    [Header("Throw Settings")]
    public float throwForce = 500f;
    public KeyCode throwKey = KeyCode.Mouse0; // Left click to throw

    [Header("Rotation Settings")]
    public KeyCode rotateKey = KeyCode.R;
    public float rotationSensitivity = 1f;

    [Header("UI")]
    public Text promptText; // UI text to show "Press E to pick up"
    public Text inventoryText; // UI text to show what you're holding
    public Text pickupNotificationText; // Shows "Picked up [item]" at bottom
    public float notificationDuration = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pickupSound;
    public AudioClip dropSound;

    [Header("Layers")]
    public string holdLayerName = "holdLayer";

    [Header("Drop Settings")]
    public float dropDistance = 1.5f; // Distance in front of player to drop
    public float dropHeightOffset = 0.5f; // Height above ground to drop

    private GameObject heldObj; // Object being held
    private Rigidbody heldObjRb; // Rigidbody of held object
    private bool canDrop = true; // Prevents dropping while rotating
    private int holdLayerNumber;
    private float notificationTimer = 0f;
    private Vector3 originalScale; // Store original scale to restore after pickup

    public float interactRange = 5f;
    public KeyCode interactKey = KeyCode.E;

    void Start()
    {
        holdLayerNumber = LayerMask.NameToLayer(holdLayerName);

        // If layer doesn't exist, use default layer (0)
        if (holdLayerNumber == -1)
        {
            holdLayerNumber = 0;
            Debug.LogWarning("holdLayer not found! Using default layer. Create 'holdLayer' in Project Settings → Tags and Layers");
        }

        // Get audio source if not assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Hide UI elements at start
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (pickupNotificationText != null)
            pickupNotificationText.gameObject.SetActive(false);

        // Create hold position if not assigned
        if (holdPos == null)
        {
            GameObject holdPosObj = new GameObject("HoldPosition");
            holdPosObj.transform.SetParent(transform);
            holdPosObj.transform.localPosition = new Vector3(0, 0, 2f);
            holdPos = holdPosObj.transform;
        }

        UpdateInventoryUI();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            // Pickup logic
            if (hit.transform.CompareTag("CanPickUp"))
            {
                // ... existing pickup code ...
            }
            // Water interact logic
            else if (hit.transform.GetComponent<WaterInteract>() != null)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    hit.transform.GetComponent<WaterInteract>().Interact();
                }
            }
        }

        // Show prompt only if not holding anything
        if (heldObj == null)
        {
            UpdatePromptUI();
        }
        else
        {
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }

        // Pickup/Drop logic
        if (Input.GetKeyDown(pickupKey))
        {
            if (heldObj == null) // Not holding anything
            {
                TryPickupWithRaycast();
            }
            else // Currently holding something
            {
                if (canDrop)
                {
                    DropObjectSafely();
                }
            }
        }

        // Handle held object
        if (heldObj != null)
        {
            MoveObject(); // Keep object at hold position
            RotateObject(); // Handle rotation

            // Throw object
            if (Input.GetKeyDown(throwKey) && canDrop)
            {
                ThrowObject();
            }
        }

        // Handle notification timer
        if (notificationTimer > 0)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0 && pickupNotificationText != null)
            {
                pickupNotificationText.gameObject.SetActive(false);
            }
        }
    }

    // Raycast from camera center for pickup
    void TryPickupWithRaycast()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickupRange))
        {
            if (hit.transform.gameObject.CompareTag("CanPickUp"))
            {
                PickUpObject(hit.transform.gameObject);
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) // Make sure object has Rigidbody
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();

            // Store original scale
            originalScale = heldObj.transform.localScale;

            heldObjRb.isKinematic = true;
            heldObj.transform.parent = holdPos.transform;

            // Ignore collision with player for all colliders
            Collider playerCollider = GetComponent<Collider>() ?? GetComponent<CharacterController>();
            Collider[] heldColliders = heldObj.GetComponentsInChildren<Collider>();
            if (playerCollider != null)
            {
                foreach (var col in heldColliders)
                    Physics.IgnoreCollision(col, playerCollider, true);
            }

            // Show pickup notification
            string itemName = heldObj.name;
            PickupItem pickupComponent = heldObj.GetComponent<PickupItem>();
            if (pickupComponent != null)
                itemName = pickupComponent.itemName;

            ShowPickupNotification("Picked up: " + itemName);

            // Play pickup sound
            if (audioSource != null && pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            UpdateInventoryUI();
            Debug.Log("Picked up: " + itemName);
        }
    }

    void DropObjectSafely()
    {
        if (heldObj == null) return;

        // Find a safe drop position
        Vector3 dropPosition = FindSafeDropPosition();

        // Unparent first
        heldObj.transform.parent = null;

        // Restore original scale
        heldObj.transform.localScale = originalScale;

        // Set position to safe drop location
        heldObj.transform.position = dropPosition;

        // CRITICAL FIX: Set collision detection mode to prevent falling through ground
        heldObjRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Re-enable physics
        heldObjRb.isKinematic = false;

        // Clear velocity to prevent unwanted movement
        heldObjRb.linearVelocity = Vector3.zero;
        heldObjRb.angularVelocity = Vector3.zero;

        // Re-enable collision with player for all colliders
        Collider playerCollider = GetComponent<Collider>() ?? GetComponent<CharacterController>();
        Collider[] heldColliders = heldObj.GetComponentsInChildren<Collider>();
        if (playerCollider != null)
        {
            foreach (var col in heldColliders)
                Physics.IgnoreCollision(col, playerCollider, false);
        }

        // Play drop sound
        if (audioSource != null && dropSound != null)
        {
            audioSource.PlayOneShot(dropSound);
        }

        Debug.Log("Dropped: " + heldObj.name);
        heldObj = null;
        UpdateInventoryUI();
    }

    Vector3 FindSafeDropPosition()
    {
        // Start from player position
        Vector3 startPos = transform.position + Vector3.up * 0.5f; // Slightly above player center
        Vector3 forwardDirection = transform.forward;

        // Try to place object in front of player
        Vector3 targetPosition = startPos + forwardDirection * dropDistance;

        // Raycast downward to find ground
        RaycastHit groundHit;
        if (Physics.Raycast(targetPosition + Vector3.up * 5f, Vector3.down, out groundHit, 10f))
        {
            // Place object slightly above ground
            targetPosition.y = groundHit.point.y + dropHeightOffset;
        }
        else
        {
            // If no ground found, use player's Y position
            targetPosition.y = transform.position.y;
        }

        // Check if position is clear (no colliders blocking)
        Collider objCollider = heldObj.GetComponent<Collider>();
        if (objCollider != null)
        {
            // Check for overlapping colliders at drop position
            Bounds objBounds = objCollider.bounds;
            Vector3 boxSize = objBounds.size;

            // If position is blocked, try alternative positions
            if (Physics.CheckBox(targetPosition, boxSize * 0.5f, heldObj.transform.rotation))
            {
                // Try positions around the player
                Vector3[] alternativePositions = {
                    startPos + transform.right * dropDistance,
                    startPos + (-transform.right) * dropDistance,
                    startPos + (-forwardDirection) * dropDistance,
                    startPos // Last resort: at player position
                };

                foreach (Vector3 altPos in alternativePositions)
                {
                    Vector3 testPos = altPos;

                    // Raycast for ground at alternative position
                    if (Physics.Raycast(testPos + Vector3.up * 5f, Vector3.down, out groundHit, 10f))
                    {
                        testPos.y = groundHit.point.y + dropHeightOffset;
                    }
                    else
                    {
                        testPos.y = transform.position.y;
                    }

                    // Check if this position is clear
                    if (!Physics.CheckBox(testPos, boxSize * 0.5f, heldObj.transform.rotation))
                    {
                        return testPos;
                    }
                }
            }
        }

        return targetPosition;
    }

    void MoveObject()
    {
        // Keep object at hold position
        heldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(rotateKey)) // Hold R to rotate
        {
            canDrop = false; // Prevent dropping while rotating

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;

            // Rotate object based on mouse movement
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        if (heldObj == null) return;

        // Find safe position first
        Vector3 throwPosition = FindSafeDropPosition();

        // Unparent and restore scale
        heldObj.transform.parent = null;
        heldObj.transform.localScale = originalScale;
        heldObj.transform.position = throwPosition;

        // CRITICAL FIX: Set collision detection mode for fast-moving objects
        heldObjRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Re-enable physics
        heldObjRb.isKinematic = false;

        // Clear velocity first, then add throw force
        heldObjRb.linearVelocity = Vector3.zero;
        heldObjRb.angularVelocity = Vector3.zero;
        heldObjRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        // Re-enable collision with player for all colliders
        Collider playerCollider = GetComponent<Collider>() ?? GetComponent<CharacterController>();
        Collider[] heldColliders = heldObj.GetComponentsInChildren<Collider>();
        if (playerCollider != null)
        {
            foreach (var col in heldColliders)
                Physics.IgnoreCollision(col, playerCollider, false);
        }

        Debug.Log("Threw: " + heldObj.name);
        heldObj = null;
        UpdateInventoryUI();
    }

    // Prompt UI only for object under crosshair
    void UpdatePromptUI()
    {
        if (promptText != null && heldObj == null)
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                if (hit.transform.gameObject.CompareTag("CanPickUp"))
                {
                    string itemName = hit.transform.gameObject.name;
                    PickupItem pickupComponent = hit.transform.gameObject.GetComponent<PickupItem>();
                    if (pickupComponent != null)
                        itemName = pickupComponent.itemName;

                    promptText.gameObject.SetActive(true);
                    promptText.text = "Press " + pickupKey.ToString().Replace("KeyCode.", "") + " to pick up " + itemName;
                    return;
                }
            }
            promptText.gameObject.SetActive(false);
        }
    }

    void UpdateInventoryUI()
    {
        if (inventoryText != null)
        {
            if (heldObj != null)
            {
                string itemName = heldObj.name;
                PickupItem pickupComponent = heldObj.GetComponent<PickupItem>();
                if (pickupComponent != null)
                    itemName = pickupComponent.itemName;

                inventoryText.text = "Holding: " + itemName + " | " + pickupKey + ":Drop | " + throwKey + ":Throw | " + rotateKey + ":Rotate";
            }
            else
            {
                inventoryText.text = "Not holding anything";
            }
        }
    }

    void ShowPickupNotification(string message)
    {
        if (pickupNotificationText != null)
        {
            pickupNotificationText.text = message;
            pickupNotificationText.gameObject.SetActive(true);
            notificationTimer = notificationDuration;
        }
    }

    // Public methods
    public bool IsHoldingSomething()
    {
        return heldObj != null;
    }

    public GameObject GetHeldObject()
    {
        return heldObj;
    }

    // Draw pickup range in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        // Draw hold position
        if (holdPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(holdPos.position, Vector3.one * 0.2f);
        }

        // Draw drop range
        Gizmos.color = Color.blue;
        Vector3 dropPos = transform.position + transform.forward * dropDistance;
        Gizmos.DrawWireCube(dropPos, Vector3.one * 0.3f);
    }
}