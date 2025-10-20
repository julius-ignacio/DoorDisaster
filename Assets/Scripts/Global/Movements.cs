using MilkShake;
using UnityEngine;
using Terresquall;

public enum FootstepSurface
{
    Pavement,
    Grass
}


[RequireComponent(typeof(CharacterController))]
public class Movements : MonoBehaviour
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

    public FootstepSurface currentSurface = FootstepSurface.Pavement;



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
    // Ground check
    isGrounded = controller.isGrounded;
    if (isGrounded && velocity.y < 0)
        velocity.y = -2f;

    // --- Input ---
    float x = VirtualJoystick.GetAxis("Horizontal");
    float z = VirtualJoystick.GetAxis("Vertical");

#if UNITY_EDITOR || UNITY_STANDALONE
    x += Input.GetAxis("Horizontal");
    z += Input.GetAxis("Vertical");
#endif

    Vector3 move = new Vector3(x, 0, z);
    if (move.magnitude > 1f) move.Normalize();

    // Apply move relative to camera (optional)
    Vector3 moveDir = transform.TransformDirection(move);
    controller.Move(moveDir * speed * Time.deltaTime);

    // --- Jump ---
    if (isGrounded && Input.GetButtonDown("Jump"))
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

    // --- Gravity ---
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);

    // --- Footsteps ---
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
            switch (currentSurface)
            {
                case FootstepSurface.Pavement:
                    AudioManager.Instance.PlaySFX(2); // pavement sound
                    break;
                case FootstepSurface.Grass:
                    AudioManager.Instance.PlaySFX(1); // grass sound
                    break;
            }

            stepTimer = walkStepInterval;
        }
    }
    else
    {
        stepTimer = 0f;
    }
}



    }

