using MilkShake;
using UnityEngine;
using Terresquall;

[RequireComponent(typeof(CharacterController))]
public class Movements : MonoBehaviour
{
    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    private CharacterController controller;
    public Vector3 velocity;
    public bool isGrounded;

    public float walkStepInterval = 0.6f;   // time between footsteps
    public float runStepInterval = 0.35f;   // faster for running
    private float stepTimer;
    public bool footstepsEnabled = true;


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
                if (  AudioManager.Instance.Clips.Length > 4)
                {
                    // play random footstep clip
                      AudioManager.Instance.PlaySFX(2); // index for footsteps in your Clips array
                    
                }
                else
                {
                      AudioManager.Instance.PlaySFX(0); // fallback sound if no footsteps assigned
                }
             

                stepTimer = walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // reset
        }
}

    }

