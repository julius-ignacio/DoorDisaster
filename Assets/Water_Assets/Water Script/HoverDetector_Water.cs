using UnityEngine;

public class HoverDetector_Water : MonoBehaviour
{
    [Header("References")]
    public FlashlightController_Water flashlightController; // Reference to flashlight controller
    public Transform playerCamera;                           // Player's camera (for raycast)

    [Header("Settings")]
    public float interactDistance = 3f;                      // Max distance to detect hover

    private bool isHovering = false;

    void Start()
    {
        // Auto-assign main camera if not manually assigned
        if (playerCamera == null)
        {
            if (Camera.main != null)
                playerCamera = Camera.main.transform;
            else
                Debug.LogWarning("HoverDetector_Water: No player camera assigned!");
        }

        if (flashlightController == null)
        {
            Debug.LogWarning("HoverDetector_Water: No FlashlightController assigned!");
        }
    }

    void Update()
    {
        if (flashlightController == null || playerCamera == null) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;
        bool hoveringNow = false;

        // Cast a ray forward to detect flashlight object
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if the ray hit the flashlight object
            if (hit.collider.gameObject == flashlightController.gameObject)
            {
                hoveringNow = true;
            }
        }

        // Started hovering
        if (hoveringNow && !isHovering)
        {
            flashlightController.ShowPickupUI(true);
            isHovering = true;
        }
        // Stopped hovering
        else if (!hoveringNow && isHovering)
        {
            flashlightController.ShowPickupUI(false);
            isHovering = false;
        }
    }
}
