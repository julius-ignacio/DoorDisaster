using UnityEngine;
using UnityEngine.UI;

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
    public float mouseSensitivity = 2f;
    public Transform playerBody;
    public Camera firstPersonCamera;

    [Header("Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepSounds;
    public float footstepInterval = 0.5f;

    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    [Header("Inventory/Items")]
    public bool hasTowel = false;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private float footstepTimer;
    private bool isMoving;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonCamera == null)
        {
            Camera foundCamera = GetComponentInChildren<Camera>();
            if (foundCamera != null)
                firstPersonCamera = foundCamera;
        }

        if (playerBody == null)
            playerBody = transform;

        // Initialize health system
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

            // Force fill color update
            Image fillImage = healthBar.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.green;
            }

            Debug.Log("Health bar initialized - Value: " + currentHealth + "/" + maxHealth);
        }

        Debug.Log("Camera found: " + (firstPersonCamera != null));
        Debug.Log("Player body: " + (playerBody != null));
    }

    void Update()
    {
        // DEBUG: Check controller state
        if (!controller.enabled)
        {
            Debug.Log("Controller is DISABLED! Player probably died.");
            return;
        }

        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleFootsteps();
        HandleCursorToggle();
    }

    void LateUpdate()
    {
        HandleMouseLook();
    }

    void HandleGroundCheck()
    {
        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        isGrounded = Physics.CheckSphere(groundCheckPos, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep grounded
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // DEBUG: Check input
        if (x != 0 || z != 0)
            Debug.Log("Input detected - X: " + x + ", Z: " + z + ", IsGrounded: " + isGrounded);

        Vector3 move = transform.right * x + transform.forward * z;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * speed * Time.deltaTime);

        isMoving = move.magnitude > 0.1f && isGrounded;
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        playerBody.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        if (firstPersonCamera != null)
            firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

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
                int randomIndex = Random.Range(0, footstepSounds.Length);
                footstepAudioSource.PlayOneShot(footstepSounds[randomIndex]);

                float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
                footstepTimer = footstepInterval * (walkSpeed / currentSpeed);
            }
        }
    }

    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // =====================
    // Health System Methods
    // =====================
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player is dead");
        controller.enabled = false; // disable movement when dead

        // Trigger Game Over
        GameOverManager.TriggerDeath(
            "HEALTH DEPLETED",
            "You were burned by the fire! Remember to plan your escape early."
        );
    }


    // =====================
    // Inventory/Item Methods
    // =====================
    public bool HasTowel()
    {
        return hasTowel;
    }

    public void SetTowel(bool value)
    {
        hasTowel = value;
    }

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
            // Add towel usage logic here
        }
        else
        {
            Debug.Log("No towel to use!");
        }
    }

    // Existing helper methods
    public bool IsGrounded() => isGrounded;
    public bool IsMoving() => isMoving;
    public float GetCurrentSpeed() => Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

    // Emergency method for testing
    public void ForceEnable()
    {
        controller.enabled = true;
        Debug.Log("Movement force enabled!");
    }

    void OnDrawGizmosSelected()
    {
        if (controller == null) return;

        Vector3 groundCheckPos = controller.bounds.center - new Vector3(0, controller.bounds.extents.y, 0);
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPos, groundDistance);
    }
}