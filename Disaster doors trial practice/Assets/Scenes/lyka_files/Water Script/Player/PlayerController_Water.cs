using UnityEngine;
using Terresquall; // ✅ For VirtualJoystick support

[RequireComponent(typeof(CharacterController))]
public class PlayerController_Water : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 20f;
    public float sprintMultiplier = 2f;
    public float floodSpeedMultiplier = 0.5f;
    public float gravity = -20f;
    public float jumpHeight = 7f;
    public float acceleration = 10f;
    public float deceleration = 12f;

    [HideInInspector] public bool canMove = true;

    [Header("Sprint Settings")]
    public bool enableSprint = true;
    public float sprintStamina = 15f;
    public float staminaRecoveryRate = 3f;
    private float currentStamina;
    private bool isSprinting;

    [Header("Oxygen System")]
    public OxygenSys_Water oxygenSys;
    public float drainInterval = 2f;
    public float refillInterval = 1f;
    private float oxygenTimer;

    [Header("Footstep Settings")]
    public AudioSource footsteps;
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.6f;
    public float sprintStepInterval = 0.4f;
    private float stepTimer;

    [Header("Ripple System")]
    public Camera rippleCamera;
    public ParticleSystem rippleParticles;
    public float rippleFollowSpeed = 5f;

    private Animator animator;
    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 moveDirection;
    private bool isGrounded;
    private bool inFlood = false;
     private GameObject respawnPoint;

    [Header("Flood Detection")]
    public Transform floodSurface;
    public float waterHeight = 0f;

    public bool IsGrounded => isGrounded;

    public void Jump()
    {
        if (isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        respawnPoint = GameObject.FindWithTag("Respawn");
        currentStamina = sprintStamina;
    }

    void Update()
    {
        if (!canMove) return;

        // -------------------------
        // GROUND CHECK
        // -------------------------
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // -------------------------
        // MOVEMENT INPUT
        // -------------------------
        float x = VirtualJoystick.GetAxis("Horizontal") + Input.GetAxis("Horizontal");
        float z = VirtualJoystick.GetAxis("Vertical") + Input.GetAxis("Vertical");

        Vector3 inputMove = new Vector3(x, 0, z);
        if (inputMove.magnitude > 1f)
            inputMove.Normalize();

        HandleSprint(inputMove.magnitude > 0.1f);

        float baseSpeed = inFlood ? speed * floodSpeedMultiplier : speed;
        float currentSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;

        Vector3 targetMove = transform.TransformDirection(inputMove) * currentSpeed;
        float accel = inputMove.magnitude > 0.1f ? acceleration : deceleration;
        moveDirection = Vector3.Lerp(moveDirection, targetMove, accel * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
            Jump();

        velocity.y += gravity * Time.deltaTime;
        Vector3 finalMove = moveDirection + new Vector3(0, velocity.y, 0);
        controller.Move(finalMove * Time.deltaTime);

        // -------------------------
        // ANIMATOR HANDLING
        // -------------------------
        if (animator != null)
        {
            animator.SetBool("IsMoving", inputMove.magnitude > 0.1f);
            animator.SetBool("IsSprinting", isSprinting);
            animator.SetFloat("MoveSpeed", controller.velocity.magnitude / (speed * sprintMultiplier));
        }

        // -------------------------
        // OTHER SYSTEMS
        // -------------------------
        HandleFootsteps(moveDirection);
        HandleOxygen();
        UpdateRippleSystem();
    }

    // -------------------------
    // SPRINT SYSTEM
    // -------------------------
    void HandleSprint(bool isMoving)
    {
        if (!enableSprint || inFlood)
        {
            isSprinting = false;
            return;
        }

        bool sprintInput = Input.GetKey(KeyCode.LeftShift);

        if (sprintInput && isMoving && currentStamina > 0f)
            isSprinting = true;
        else
            isSprinting = false;

        if (isSprinting)
            currentStamina -= Time.deltaTime;
        else
            currentStamina += staminaRecoveryRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, sprintStamina);
    }

    public void StartSprint() => isSprinting = true;
    public void StopSprint() => isSprinting = false;

    // -------------------------
    // FOOTSTEPS SYSTEM
    // -------------------------
    void HandleFootsteps(Vector3 move)
    {
        if (footsteps == null) return;

        if (isGrounded && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            float interval = isSprinting ? sprintStepInterval : walkStepInterval;

            if (stepTimer <= 0f)
            {
                if (footstepClips.Length > 0)
                    footsteps.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                else
                    footsteps.PlayOneShot(footsteps.clip);
                stepTimer = interval;
            }
        }
        else stepTimer = 0f;
    }

    // -------------------------
    // OXYGEN SYSTEM
    // -------------------------
    void HandleOxygen()
    {
        if (oxygenSys == null) return;

        oxygenTimer -= Time.deltaTime;

        if (inFlood)
        {
            if (oxygenTimer <= 0f)
            {
                oxygenSys.UseOxygen(1);
                oxygenTimer = drainInterval;
            }
        }
        else
        {
            if (oxygenSys.currentOxygen < oxygenSys.maxOxygen && oxygenTimer <= 0f)
            {
                oxygenSys.RefillOxygen(1);
                oxygenTimer = refillInterval;
            }
        }

        if (oxygenSys.currentOxygen <= 0 && respawnPoint != null)
        {
            transform.position = respawnPoint.transform.position;
            oxygenSys.RefillOxygen(oxygenSys.maxOxygen);
            velocity = Vector3.zero;
        }
    }

    // -------------------------
    // RIPPLE SYSTEM
    // -------------------------
    void UpdateRippleSystem()
    {
        if (rippleCamera != null)
        {
            Vector3 targetPos = new Vector3(transform.position.x, rippleCamera.transform.position.y, transform.position.z);
            rippleCamera.transform.position = Vector3.Lerp(rippleCamera.transform.position, targetPos, rippleFollowSpeed * Time.deltaTime);
        }

        if (rippleParticles != null)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
            {
                Vector3 target = hit.point + Vector3.up * 0.05f;
                rippleParticles.transform.position = Vector3.Lerp(rippleParticles.transform.position, target, 8f * Time.deltaTime);
            }
        }
    }

    // -------------------------
    // FLOOD DETECTION
    // -------------------------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flood"))
            inFlood = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Flood"))
            inFlood = false;
    }
}
