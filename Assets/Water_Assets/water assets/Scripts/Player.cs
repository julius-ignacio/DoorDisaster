using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles player movement, camera control, and underwater behavior.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    #region === Inspector Variables ===

    [Header("Movement Settings")]
    [Tooltip("Base movement speed of the player.")]
    public float moveSpeed = 6f;

    [Tooltip("Gravity strength applied to the player.")]
    public float gravity = 9.81f;

    [Tooltip("Jump height of the player.")]
    public float jumpHeight = 2f;

    [Header("Underwater Settings")]
    [Tooltip("Movement speed multiplier when underwater.")]
    [SerializeField] private float waterSpeedMultiplier = 0.5f;

    [Tooltip("Jump force multiplier when underwater.")]
    [SerializeField] private float waterJumpMultiplier = 0.4f;

    [Tooltip("Gravity multiplier when underwater.")]
    [SerializeField] private float waterGravityMultiplier = 0.3f;

    [Header("Camera Settings")]
    [Tooltip("Cameraman pivot for camera rotation.")]
    public GameObject cameraman;

    [Tooltip("Ripple camera reference for effects.")]
    public GameObject rippleCamera;

    [SerializeField, Tooltip("Minimum vertical camera angle.")]
    private float cameraMinY = -45f;

    [SerializeField, Tooltip("Maximum vertical camera angle.")]
    private float cameraMaxY = 45f;

    [SerializeField, Tooltip("Minimum camera zoom distance.")]
    private float cameraDistanceMin = -12f;

    [SerializeField, Tooltip("Maximum camera zoom distance.")]
    private float cameraDistanceMax = -4f;

    [Header("Ripple Effect")]
    [Tooltip("Particle system for water ripple effects.")]
    public ParticleSystem ripple;

    #endregion

    #region === Private Variables ===

    private CharacterController characterController;
    private float cameraPitch = 0f;
    private float verticalVelocity = 0f;
    private float zoom = -7f;
    private Vector3 lastPosition;
    private bool isInWater = false;

    // Debug velocity
    [SerializeField, Tooltip("Horizontal movement velocity for debugging.")]
    private float velocityXZ;

    [SerializeField, Tooltip("Vertical movement velocity for debugging.")]
    private float velocityY;

    // Raycast hit info
    private RaycastHit groundHit;

    #endregion

    #region === Unity Methods ===

    private void Start()
    {
        Application.targetFrameRate = 60;

        characterController = GetComponent<CharacterController>();
        lastPosition = transform.position;

        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleCamera();
        HandleInput();

        UpdateVelocity();
        UpdateRipplePosition();

        // Pass player position to shaders
        Shader.SetGlobalVector("_Player", transform.position);
    }

    #endregion

    #region === Movement Logic ===

    /// <summary>
    /// Handles walking, gravity, and water-based movement.
    /// </summary>
    private void HandleMovement()
    {
        Vector3 moveInput = GetCameraRelativeMovement();

        // Adjust speed for water and sprinting
        float currentSpeed = moveSpeed * (isInWater ? waterSpeedMultiplier : 1f);
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed *= 2f;

        // Horizontal movement
        if (moveInput.magnitude > 0f)
        {
            characterController.Move(moveInput.normalized * currentSpeed * Time.deltaTime * 10f);
            transform.forward = moveInput.normalized;
        }

        ApplyGravity();
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);

        CheckGroundStatus();
        CheckWaterStatus();

        // Toggle ripple based on water state
        ripple.gameObject.SetActive(isInWater);
    }

    /// <summary>
    /// Creates movement vector based on camera orientation.
    /// </summary>
    private Vector3 GetCameraRelativeMovement()
    {
        Vector3 camRight = cameraman.transform.right;
        Vector3 camForward = cameraman.transform.forward;

        camRight.y = 0f;
        camForward.y = 0f;

        return (camRight.normalized * Input.GetAxis("Horizontal")) +
               (camForward.normalized * Input.GetAxis("Vertical"));
    }

    /// <summary>
    /// Applies gravity and underwater adjustments.
    /// </summary>
    private void ApplyGravity()
    {
        float adjustedGravity = gravity * (isInWater ? waterGravityMultiplier : 1f);

        verticalVelocity -= adjustedGravity * Time.deltaTime * 5f;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -99f, verticalVelocity);

        // Reset small downward velocity when grounded
        if (groundHit.collider && verticalVelocity < -2f)
            verticalVelocity = -2f;
    }

    /// <summary>
    /// Handles jump logic.
    /// </summary>
    private void HandleJump()
    {
        if (verticalVelocity > 2f) return; // Prevent double jumps

        float jumpForce = jumpHeight * (isInWater ? waterJumpMultiplier : 1f);
        verticalVelocity = Mathf.Sqrt(jumpForce * gravity * 2f);
        characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    #endregion

    #region === Camera Logic ===

    /// <summary>
    /// Handles camera rotation and zooming.
    /// </summary>
    private void HandleCamera()
    {
        cameraman.transform.position = transform.position;

        // Vertical camera rotation (pitch)
        cameraPitch -= Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * 300f;
        cameraPitch = Mathf.Clamp(cameraPitch, cameraMinY, cameraMaxY);

        // Horizontal camera rotation (yaw)
        cameraman.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150f);

        // Zoom
        zoom += Input.mouseScrollDelta.y * Time.fixedDeltaTime * 100f;
        zoom = Mathf.Clamp(zoom, cameraDistanceMin, cameraDistanceMax);

        // Apply final camera transform
        cameraman.transform.eulerAngles = new Vector3(cameraPitch, cameraman.transform.eulerAngles.y, 0f);
        cameraman.transform.GetChild(0).localPosition = new Vector3(0f, 1.15f, zoom);
    }

    #endregion

    #region === Ripple Logic ===

    /// <summary>
    /// Updates ripple effect position based on movement and ground detection.
    /// </summary>
    private void UpdateRipplePosition()
    {
        ripple.transform.position = groundHit.collider
            ? transform.position + transform.forward
            : transform.position;
    }

    /// <summary>
    /// Creates ripple bursts when entering/exiting water.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4) // Water layer
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4) // Water layer
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
    }

    #endregion

    #region === Environment Checks ===

    /// <summary>
    /// Checks if the player is on the ground.
    /// </summary>
    private void CheckGroundStatus()
    {
        Physics.Raycast(transform.position, Vector3.down, out groundHit, 2.7f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(transform.position, Vector3.down * 2.7f, Color.yellow);
    }

    /// <summary>
    /// Checks if the player is currently in water.
    /// </summary>
    private void CheckWaterStatus()
    {
        float height = characterController.height + characterController.radius;
        isInWater = Physics.Raycast(
            transform.position + Vector3.up * height,
            Vector3.down,
            height * 2f,
            LayerMask.GetMask("Water")
        );

        Debug.DrawRay(transform.position + Vector3.up * height, Vector3.down * height, Color.blue);
    }

    #endregion

    #region === Utility ===

    /// <summary>
    /// Updates velocity calculations for debugging or animations.
    /// </summary>
    private void UpdateVelocity()
    {
        velocityXZ = Vector3.Distance(
            new Vector3(transform.position.x, 0f, transform.position.z),
            new Vector3(lastPosition.x, 0f, lastPosition.z)
        );

        velocityY = Mathf.Abs(transform.position.y - lastPosition.y);
        lastPosition = transform.position;
    }

    /// <summary>
    /// Creates a circular ripple effect burst.
    /// </summary>
    private void CreateRipple(int start, int end, int step, float speed, float size, float lifetime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = start;
        ripple.transform.eulerAngles = forward;

        for (int i = start; i < end; i += step)
        {
            ripple.Emit(
                transform.position + ripple.transform.forward * 1.15f,
                ripple.transform.forward * speed,
                size,
                lifetime,
                Color.white
            );

            ripple.transform.Rotate(Vector3.up * step, Space.World);
        }
    }

    /// <summary>
    /// Handles basic player inputs like jumping and quitting.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) HandleJump();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    #endregion
}
