using UnityEngine;
using UnityEngine.UI;
using Terresquall;


[RequireComponent(typeof(CharacterController))]
public class Movements2 : MonoBehaviour
{
    public float speed = 3f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    private CharacterController controller;
    public Vector3 velocity;
    public bool isGrounded;

    public float walkStepInterval = 0.6f;   // time between footsteps
    public float runStepInterval = 0.35f;   // faster for running
    private float stepTimer;
    public bool footstepsEnabled = true;
  //  public GameObject signalSwitchFootstepsSfx;


    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;

    [Header("Inventory / Items")]
    public bool hasTowel = false;



    void Start()
    {
        controller = GetComponent<CharacterController>();
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

        // Check if on ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement inputs
        float x = VirtualJoystick.GetAxis("Horizontal") + Input.GetAxis("Horizontal");
        float z = VirtualJoystick.GetAxis("Vertical") + Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);

        if (move.magnitude > 1f) move.Normalize();

        // Apply movement
        controller.Move(transform.TransformDirection(move) * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Footsteps ---
        HandleFootsteps(move);
    }

    void HandleFootsteps(Vector3 move)
    {
        if (!footstepsEnabled) return; // 🚫 disable footsteps entirely
        if (isGrounded && move.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                         AudioManager.Instance.PlaySFX(2); // fallback sound if no footsteps assigned



                stepTimer = walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // reset
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
}