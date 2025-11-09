using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 5f;
    public Transform holdPosition; // Position relative to camera where objects are held
    public LayerMask pickupMask = -1; // What layers can be picked up

    [Header("Debug")]
    public bool showDebugMessages = true; // Toggle debug messages on/off

    private Camera playerCamera; // Reference to the nested camera
    private GameObject heldObject;
    private Rigidbody heldObjectRB;

    void Start()
    {
        // Get the camera component from the nested camera
        playerCamera = GetComponentInChildren<Camera>();

        // Safety check
        if (playerCamera == null)
        {
            Debug.LogError("No camera found as child of " + gameObject.name + "! Make sure camera is nested under player.");
        }

        if (holdPosition == null)
        {
            Debug.LogError("Hold Position not assigned! Create an empty GameObject as child of camera and assign it.");
        }
    }

    void Update()
    {
        // Handle pickup/drop input
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
                TryPickup();
            else
                DropObject();
        }
    }

    void TryPickup()
    {
        // Cast a ray from the center of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupMask))
        {
            if (showDebugMessages)
                Debug.Log("Hit object: " + hit.collider.name + " with tag: " + hit.collider.tag);

            // Check if the object can be picked up
            if (hit.collider.CompareTag("Pickupable"))
            {
                PickupObject(hit.collider.gameObject);
            }
            else
            {
                if (showDebugMessages)
                    Debug.Log("Object '" + hit.collider.name + "' is not pickupable! Make sure it has the 'Pickupable' tag.");
            }
        }
        else
        {
            if (showDebugMessages)
                Debug.Log("No object hit by raycast within range of " + pickupRange + " units.");
        }
    }

    void PickupObject(GameObject obj)
    {
        if (showDebugMessages)
            Debug.Log("Picking up: " + obj.name);

        // Store references
        heldObject = obj;
        heldObjectRB = obj.GetComponent<Rigidbody>();

        // Disable physics while held
        if (heldObjectRB != null)
        {
            heldObjectRB.isKinematic = true; // Disable physics simulation
            heldObjectRB.useGravity = false; // Disable gravity
        }

        // Parent to hold position and reset local transform
        obj.transform.SetParent(holdPosition);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    void DropObject()
    {
        if (showDebugMessages)
            Debug.Log("Dropping: " + heldObject.name);

        // Re-enable physics
        if (heldObjectRB != null)
        {
            heldObjectRB.isKinematic = false; // Re-enable physics simulation
            heldObjectRB.useGravity = true;   // Re-enable gravity

            // Add a small downward force to help it drop naturally
            heldObjectRB.AddForce(Vector3.down * 2f, ForceMode.Impulse);

            // Optional: Add some forward velocity based on camera direction
            Vector3 throwDirection = playerCamera.transform.forward * 2f;
            heldObjectRB.AddForce(throwDirection, ForceMode.VelocityChange);
        }

        // Unparent from hold position
        heldObject.transform.SetParent(null);

        // Clear references
        heldObject = null;
        heldObjectRB = null;
    }

    // Optional: Visual debug for pickup range
    void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * pickupRange);
        }
    }
}