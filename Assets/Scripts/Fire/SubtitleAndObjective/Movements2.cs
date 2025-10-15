using UnityEngine;
using UnityEngine.UI;
using PinePie.SimpleJoystick; // Import the namespace

[RequireComponent(typeof(CharacterController))]
public class Movements2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Camera Settings")]
    public Transform playerBody;
    public Camera firstPersonCamera;
    public float lookSensitivity = 2f;

    [Header("Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;

    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    [Header("Inventory / Items")]
    public bool hasTowel = false;

    [Header("Mobile Controls")]
    public bool isMobile = false;
    public JoystickController movementJoystick;  // Left joystick for movement
    public JoystickController lookJoystick;      // Right joystick for camera (optional)
    public Button jumpButton;                     // Jump button
    public float mobileLookSensitivity = 0.5f;   // Separate sensitivity for mobile

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private float footstepTimer;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (firstPersonCamera == null)
            firstPersonCamera = GetComponentInChildren<Camera>();

        if (playerBody == null)
            playerBody = transform;

        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // Add jump button listener for mobile
        if (isMobile && jumpButton != null)
            jumpButton.onClick.AddListener(Jump);

        // Lock cursor for PC
        if (!isMobile)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleLook();
        HandleFootsteps();
    }

    void HandleGroundCheck()
    {
        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        isGrounded = Physics.CheckSphere(groundCheckPos, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        if (isMobile && movementJoystick != null)
        {
            // Get input from mobile joystick
            Vector2 dir = movementJoystick.InputDirection;
            x = dir.x;
            z = dir.y;
        }
        else
        {
            // PC input
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Running only on PC (shift key)
        float speed = (!isMobile && Input.GetKey(KeyCode.LeftShift)) ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);
        isMoving = move.magnitude > 0.1f && isGrounded;
    }

    void HandleLook()
    {
        if (isMobile)
        {
            // Option 1: Using a look joystick (recommended)
            if (lookJoystick != null)
            {
                Vector2 lookDir = lookJoystick.InputDirection;

                if (lookDir.magnitude > 0.1f) // Only rotate if joystick is being used
                {
                    float mouseX = lookDir.x * mobileLookSensitivity;
                    float mouseY = lookDir.y * mobileLookSensitivity;

                    playerBody.Rotate(Vector3.up * mouseX);

                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                }
            }
            // Option 2: Using touch on right side of screen (fallback)
            else if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Only use touch on right side of screen for looking
                if (touch.position.x > Screen.width / 2 && touch.phase == TouchPhase.Moved)
                {
                    float mouseX = touch.deltaPosition.x * mobileLookSensitivity * 0.1f;
                    float mouseY = touch.deltaPosition.y * mobileLookSensitivity * 0.1f;

                    playerBody.Rotate(Vector3.up * mouseX);

                    xRotation -= mouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                    firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                }
            }
        }
        else
        {
            // PC mouse look
            float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

            playerBody.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleJump()
    {
        // PC jump
        if (!isMobile && Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        ApplyGravity();
    }

    // Mobile jump (called by button)
    void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleFootsteps()
    {
        if (isMoving && footstepAudioSource != null && footstepSounds.Length > 0)
        {
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                int index = Random.Range(0, footstepSounds.Length);
                footstepAudioSource.PlayOneShot(footstepSounds[index]);

                float speed = isMobile ? walkSpeed : (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
                footstepTimer = footstepInterval * (walkSpeed / speed);
            }
        }
    }

    // ===================== Health System =====================
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        AudioManager.Instance.PlaySFX(16);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthBar != null) healthBar.value = currentHealth;

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        controller.enabled = false;
        Debug.Log("Player died!");
        GameOverManager.TriggerDeath("HEALTH DEPLETED", "You were burned by the fire!");
    }

    // ===================== Inventory / Items =====================
    public bool HasTowel() => hasTowel;
    public void SetTowel(bool value) => hasTowel = value;

    public void PickupTowel()
    {
        hasTowel = true;
        Debug.Log("Picked up towel!");
    }

    public void UseTowel()
    {
        if (hasTowel)
        {
            hasTowel = false;
            Debug.Log("Used towel!");
        }
        else Debug.Log("No towel to use!");
    }

    public void ForceEnable()
    {
        controller.enabled = true;
        Debug.Log("Movement force enabled!");
    }

    public bool IsGrounded() => isGrounded;
    public bool IsMoving() => isMoving;

    void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPos, groundDistance);
    }
}