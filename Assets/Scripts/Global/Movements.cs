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

[Header("Camera Bob Settings")]
public Camera playerCamera;            // Assign your camera here
public float bobAmplitude = 0.05f;     // up/down bob size
public float bobFrequency = 6f;        // how fast the bob cycles
public float tiltAmplitude = 1.5f;     // left/right tilt angle
public float tiltFrequency = 3f;       // how fast it tilts
private float bobTimer = 0f;
private Vector3 cameraInitialPos;
private Quaternion cameraInitialRot;





    void Start()
    {
        controller = GetComponent<CharacterController>();
            if (playerCamera != null)
    {
        cameraInitialPos = playerCamera.transform.localPosition;
        cameraInitialRot = playerCamera.transform.localRotation;
    }
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

   HandleHeadBob(move);


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


void HandleHeadBob(Vector3 move)
{
    if (playerCamera == null) return;

    bool isMoving = move.magnitude > 0.1f && isGrounded;

    if (isMoving)
    {
        bobTimer += Time.deltaTime * bobFrequency;

        // Up-down bob
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;

        // Left-right tilt (like head sway)
        float tiltAngle = Mathf.Sin(bobTimer * tiltFrequency) * tiltAmplitude;

        // Apply new position
        Vector3 targetPos = cameraInitialPos + new Vector3(0, bobOffset, 0);
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            targetPos,
            Time.deltaTime * 8f
        );

        // Apply tilt rotation
        Quaternion targetRot = cameraInitialRot * Quaternion.Euler(0, 0, tiltAngle);
        playerCamera.transform.localRotation = Quaternion.Lerp(
            playerCamera.transform.localRotation,
            targetRot,
            Time.deltaTime * 8f
        );
    }
    else
    {
        // Smoothly reset to neutral when standing still
        playerCamera.transform.localPosition = Vector3.Lerp(
            playerCamera.transform.localPosition,
            cameraInitialPos,
            Time.deltaTime * 5f
        );

        playerCamera.transform.localRotation = Quaternion.Lerp(
            playerCamera.transform.localRotation,
            cameraInitialRot,
            Time.deltaTime * 5f
        );

        bobTimer = 0f;
    }
}





}

