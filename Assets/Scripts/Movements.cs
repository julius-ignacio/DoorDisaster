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


    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Awake()
    {
        GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject Cube in Cubes)
        {
            // Change color of all cubes to blue){
            Cube.GetComponent<Renderer>().material.color = Color.red; // Change color of Cube
        }

        //   DoorTrialBtn.SetActive(false); // Hide the DoorTrialBtn at the start
    }

void Update()
{
    // Check if on ground
    isGrounded = controller.isGrounded;
    if (isGrounded && velocity.y < 0)
    {
        velocity.y = -2f; // small downward force to stick to ground
    }

    // Get inputs from BOTH joystick and keyboard
    float x = VirtualJoystick.GetAxis("Horizontal") + Input.GetAxis("Horizontal");
    float z = VirtualJoystick.GetAxis("Vertical")   + Input.GetAxis("Vertical");

    // Normalize if both inputs are pressed (prevents double speed)
    Vector3 move = new Vector3(x, 0, z);
    if (move.magnitude > 1f)
        move.Normalize();

    // Movement relative to player
    controller.Move(transform.TransformDirection(move) * speed * Time.deltaTime);

    // Jump (keyboard only unless you add a UI button later)
    if (Input.GetButtonDown("Jump") && isGrounded)
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // Gravity
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);
}

















}
