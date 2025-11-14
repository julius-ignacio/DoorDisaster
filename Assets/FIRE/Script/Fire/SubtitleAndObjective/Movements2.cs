using UnityEngine;
using UnityEngine.UI;
using Terresquall;

[RequireComponent(typeof(CharacterController))]
public class Movements2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    [Header("Sprint Settings")]
    public Button sprintButton; // Assign in Inspector
    private bool isSprinting = false;
    private bool sprintFromButton = false;
    private float currentSpeed;

    private CharacterController controller;
    public Vector3 velocity;
    public bool isGrounded;

    [Header("Footsteps")]
    public float walkStepInterval = 0.6f;
    public float runStepInterval = 0.35f;
    private float stepTimer;
    public bool footstepsEnabled = true;

    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    [Header("Inventory / Items")]
    public bool hasTowel = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;

        if (sprintButton != null)
        {
            StartCoroutine(SetupSprintButton());
        }
        else
        {
            Debug.LogError("❌ Sprint button is NOT assigned in Inspector!");
        }
    }

    System.Collections.IEnumerator SetupSprintButton()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("✅ Setting up sprint button: " + sprintButton.name);

        UnityEngine.EventSystems.EventTrigger trigger = sprintButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = sprintButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            Debug.Log("🔧 Created new EventTrigger");
        }
        else
        {
            Debug.Log("📋 EventTrigger already exists");
        }

        trigger.triggers.Clear();

        var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => {
            Debug.Log("🖱️ PointerDown detected!");
            StartSprintFromButton();
        });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => {
            Debug.Log("🖱️ PointerUp detected!");
            StopSprintFromButton();
        });
        trigger.triggers.Add(pointerUp);

        Debug.Log("➕ EventTrigger setup complete with " + trigger.triggers.Count + " triggers");
    }

    void Awake()
    {
        GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject Cube in Cubes)
        {
            Cube.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void Update()
    {
        if (controller == null || !controller.enabled)
            return;

        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (shiftHeld)
        {
            if (!isSprinting)
            {
                sprintFromButton = false;
                StartSprint();
            }
        }
        else if (!sprintFromButton)
        {
            if (isSprinting) StopSprint();
        }

        float x = VirtualJoystick.GetAxis("Horizontal") + Input.GetAxis("Horizontal");
        float z = VirtualJoystick.GetAxis("Vertical") + Input.GetAxis("Vertical");
        Vector3 move = new Vector3(x, 0, z);
        if (move.magnitude > 1f) move.Normalize();

        Vector3 direction = new Vector3(transform.forward.x, 0, transform.forward.z).normalized * move.z +
                            new Vector3(transform.right.x, 0, transform.right.z).normalized * move.x;
        controller.Move(direction * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleFootsteps(move);
    }

    void HandleFootsteps(Vector3 move)
    {
        if (!footstepsEnabled) return;

        if (isGrounded && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                AudioManager.Instance.PlaySFX(2);
                stepTimer = isSprinting ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    public void StartSprint()
    {
        isSprinting = true;
        currentSpeed = sprintSpeed;
        Debug.Log("🟢 Started sprinting! Speed: " + currentSpeed + " via " + (sprintFromButton ? "Button" : "Keyboard"));
    }

    public void StopSprint()
    {
        isSprinting = false;
        currentSpeed = walkSpeed;
        Debug.Log("🔴 Stopped sprinting! Speed: " + currentSpeed);
    }

    public void StartSprintFromButton()
    {
        sprintFromButton = true;
        StartSprint();
    }

    public void StopSprintFromButton()
    {
        sprintFromButton = false;
        StopSprint();
    }

    public bool IsSprinting()
    {
        return isSprinting;
    }

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
}
