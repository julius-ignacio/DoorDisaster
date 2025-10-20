using UnityEngine;
using MilkShake;
using Terresquall;

[RequireComponent(typeof(CharacterController))]
public class PlayerController_Water : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 5f; // increased for smoother PC movement
    public float floodSpeedMultiplier = 1f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;
    public float smoothTime = 0.1f; // for smooth movement

    [HideInInspector] public bool canMove = true;

    [Header("Oxygen System")]
    public OxygenSys_Water oxygenSys;
    public float drainInterval = 2f;
    public float refillInterval = 1f;
    private float oxygenTimer;

    [Header("Footstep Settings")]
    public AudioSource footsteps;
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.6f;
    private float stepTimer;

    [Header("Ripple System")]
    public Camera rippleCamera;
    public ParticleSystem rippleParticles;
    public float rippleFollowSpeed = 5f;

    [Header("Joystick Settings")]
    public VirtualJoystick joystick; // ✅ Add this field for mobile controls

    private Animator animator;
    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 currentVelocity; // for SmoothDamp
    private bool isGrounded;
    private bool inFlood = false;

    private GameObject respawnPoint;

    // ---------------------------
    // PUBLIC ACCESSORS
    // ---------------------------
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

        // ✅ Try auto-assign joystick if not manually linked in Inspector
        if (joystick == null)
            joystick = FindObjectOfType<VirtualJoystick>();
    }

    void Update()
    {
        if (!canMove) return;

        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        // --- INPUT ---
        float x = 0f;
        float z = 0f;

        // ✅ Joystick input (for Android/mobile)
        if (joystick != null)
        {
x = VirtualJoystick.GetAxis("Horizontal");
z = VirtualJoystick.GetAxis("Vertical");

        }

        // ✅ Keyboard input (for PC / Editor testing)
#if UNITY_EDITOR || UNITY_STANDALONE
        x += Input.GetAxis("Horizontal");
        z += Input.GetAxis("Vertical");
#endif

        // --- MOVEMENT ---
        float currentSpeed = inFlood ? speed * floodSpeedMultiplier : speed;
        Vector3 targetMove = (transform.right * x + transform.forward * z) * currentSpeed;

        // Smooth movement
        Vector3 move = Vector3.SmoothDamp(Vector3.zero, targetMove, ref currentVelocity, smoothTime);

        // Jump (keyboard)
        if (Input.GetButtonDown("Jump"))
            Jump();

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine movement with gravity
        Vector3 finalMove = move + new Vector3(0, velocity.y, 0);

        // Move the player
        controller.Move(finalMove * Time.deltaTime);

        // Animator
        if (animator != null) animator.SetBool("IsMoving", move.magnitude > 0.1f);

        // Footsteps, Oxygen, Ripple
        HandleFootsteps(move);
        HandleOxygen();
        UpdateRippleSystem();
    }

    void HandleFootsteps(Vector3 move)
    {
        if (footsteps == null) return;

        if (isGrounded && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                if (footstepClips.Length > 0)
                    footsteps.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)]);
                else
                    footsteps.PlayOneShot(footsteps.clip);
                stepTimer = walkStepInterval;
            }
        }
        else stepTimer = 0f;
    }

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
            velocity = Vector3.zero; // Reset velocity after respawn
        }
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flood")) inFlood = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Flood")) inFlood = false;
    }
}
