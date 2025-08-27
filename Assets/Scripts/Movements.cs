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

        // // Movement only (joystick)
        // float x = VirtualJoystick.GetAxis("Horizontal"); // A/D or Left/Right
        // float z = VirtualJoystick.GetAxis("Vertical");   // W/S or Up/Down

        // Movement only (keys)
        float x = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float z = Input.GetAxis("Vertical");   // W/S or Up/Down



        // Movement relative to player, NOT camera
        Vector3 move = new Vector3(x, 0, z);
        controller.Move(transform.TransformDirection(move) * speed * Time.deltaTime);




        // // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        



    }















}
