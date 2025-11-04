using UnityEngine;

public class MouseMovement_Water : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;

    [Header("Camera Collision Settings")]
    public Transform player;           // 🎯 Player or camera anchor
    public float cameraDistance = 3f;  // Default distance from player
    public float minDistance = 0.3f;   // Minimum distance to avoid clipping
    public float smoothSpeed = 10f;    // Smoothing speed for camera reposition
    public LayerMask collisionMask;    // Which layers block the camera

    private float xRotation = 0f;
    private float yRotation = 0f;

    [HideInInspector] public bool canLook = true; // Toggle to enable/disable camera look

    private float currentDistance; // current distance adjusted dynamically

    void Start()
    {
        // ✅ Cursor stays visible and free
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentDistance = cameraDistance;
    }

    void Update()
    {
        if (!canLook) return;

        // ✅ Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        yRotation += mouseX;

        // ✅ Apply rotation to camera (KEEP THIS)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // ✅ Apply camera collision after rotation
        HandleCameraCollision();
    }

    // ✅ Prevents camera from seeing through walls
    void HandleCameraCollision()
    {
        Vector3 desiredPosition = player.position - transform.forward * cameraDistance;

        if (Physics.Linecast(player.position, desiredPosition, out RaycastHit hit, collisionMask))
        {
            currentDistance = Mathf.Clamp(hit.distance * 0.9f, minDistance, cameraDistance);
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, cameraDistance, Time.deltaTime * smoothSpeed);
        }

        Vector3 newPosition = player.position - transform.forward * currentDistance;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);
    }

    // ✅ Toggle camera look (cursor always visible)
    public void SetLookEnabled(bool value)
    {
        canLook = value;
    }
}
