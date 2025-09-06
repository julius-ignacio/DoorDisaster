using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movements : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f; // More reasonable sensitivity
    public Transform playerBody;
    public Camera firstPersonCamera;

    [Header("Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private float footstepTimer;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock cursor to center of screen
        Cursor.lockState = CursorLockMode.Locked;

        // If no camera assigned, try to find the actual Camera component (not just GameObject)
        if (firstPersonCamera == null)
        {
            Camera foundCamera = GetComponentInChildren<Camera>();
            if (foundCamera != null)
                firstPersonCamera = foundCamera;
        }

        // If no player body assigned, use this transform
        if (playerBody == null)
            playerBody = transform;

        // Debug info
        Debug.Log("Camera found: " + (firstPersonCamera != null));
        if (firstPersonCamera != null)
            Debug.Log("Camera name: " + firstPersonCamera.name);
        Debug.Log("Player body: " + (playerBody != null));
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleFootsteps();
        HandleCursorToggle();
    }

    void LateUpdate()
    {
        // Handle mouse look in LateUpdate to avoid conflicts with movement
        HandleMouseLook();
    }

    void HandleGroundCheck()
    {
        // Use controller bounds for ground check (more stable than fixed offset)
        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        isGrounded = Physics.CheckSphere(groundCheckPos, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
    }

    void HandleMovement()
    {
        // Get input
        float x = Input.GetAxis("Horizontal"); // A/D keys
        float z = Input.GetAxis("Vertical");   // W/S keys

        // Calculate movement direction relative to where we're looking
        Vector3 move = transform.right * x + transform.forward * z;

        // Determine speed (walk or run)
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Move the character
        controller.Move(move * speed * Time.deltaTime);

        // Check if we're moving for footsteps
        isMoving = move.magnitude > 0.1f && isGrounded;
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player horizontally (Y axis)
        playerBody.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically (X axis)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (firstPersonCamera != null)
        {
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleJump()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity only if not grounded (fix jitter)
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootsteps()
    {
        if (isMoving && footstepAudioSource != null && footstepSounds.Length > 0)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                // Play random footstep sound
                int randomIndex = Random.Range(0, footstepSounds.Length);
                footstepAudioSource.PlayOneShot(footstepSounds[randomIndex]);

                // Reset timer (faster for running)
                float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
                footstepTimer = footstepInterval * (walkSpeed / currentSpeed);
            }
        }
    }

    void HandleCursorToggle()
    {
        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    // Public methods for other scripts
    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public float GetCurrentSpeed()
    {
        return Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
    }

    // Draw ground check sphere in scene view
    void OnDrawGizmosSelected()
    {
        if (controller == null) return;

        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPos, groundDistance);
    }
}