using UnityEngine;
using TMPro;

public class InteractionRaycast : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactDistance = 3f;
    public Transform holdPoint;
    public float pickupForce = 150f;
    public LayerMask pickupLayer; // <- Assign your Pickupable layer in Inspector

    [Header("UI")]
    public TextMeshProUGUI interactionText; // Assign in inspector

    private Camera playerCamera;
    private Rigidbody heldObject;

    void Start()
    {
        playerCamera = Camera.main;

        if (holdPoint == null)
        {
            GameObject holdObj = new GameObject("HoldPoint");
            holdObj.transform.SetParent(playerCamera.transform);
            holdObj.transform.localPosition = new Vector3(0, 0, 2f);
            holdPoint = holdObj.transform;
        }

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Show UI if looking at pickup object
        CheckForPickupTarget();

        // Pickup / drop with E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickupObject();
            else
                DropObject();
        }

        // Drop with Q
        if (Input.GetKeyDown(KeyCode.Q) && heldObject != null)
        {
            DropObject();
        }
    }

    void FixedUpdate()
    {
        if (heldObject != null)
        {
            Vector3 moveDirection = (holdPoint.position - heldObject.position);
            heldObject.linearVelocity = moveDirection * pickupForce * Time.fixedDeltaTime;
        }
    }

    void CheckForPickupTarget()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Only check objects on pickupLayer
        if (Physics.Raycast(ray, out hit, interactDistance, pickupLayer))
        {
            if (hit.collider.CompareTag("CanPickUp"))
            {
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(true);
                    interactionText.text = "[E] Pick up " + hit.collider.name;
                }
                return;
            }
        }

        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void TryPickupObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Only hit pickupLayer
        if (Physics.Raycast(ray, out hit, interactDistance, pickupLayer))
        {
            if (hit.collider.CompareTag("CanPickUp"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    heldObject = rb;
                    heldObject.useGravity = false;
                    heldObject.linearDamping = 10;
                    Debug.Log("Picked up: " + heldObject.name);
                }
            }
        }
    }

    void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 1;
            heldObject = null;
            Debug.Log("Dropped object");
        }
    }
}
