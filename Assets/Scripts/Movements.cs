using MilkShake;
using UnityEditor.ShaderGraph;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movements : MonoBehaviour
{

    public Shaker shaker;
    public ShakePreset shakePreset;



    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private GameObject Cube, Respawn, Finish;

    // Optional: For mobile joystick
    // public Joystick joystick;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Respawn = GameObject.FindWithTag("Respawn");

        Finish = GameObject.FindWithTag("Finish");


    }

    void Awake()
    {
        GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject Cube in Cubes)
        {
            // Change color of all cubes to blue){
            Cube.GetComponent<Renderer>().material.color = Color.red; // Change color of Cube
        }
    }

    void Update()
    {
        // Check if on ground
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // small downward force to stick to ground
        }

        // // Get input (Keyboard or Virtual Joystick)
        float x = Input.GetAxis("Vertical"); // For keyboard: A/D or arrow keys
        float z = Input.GetAxis("Horizontal");   // For keyboard: W/S or arrow keys

        // For Joystick (uncomment if using joystick)
        // float x = joystick.Horizontal;
        // float z = joystick.Vertical;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);


        // Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // controller.Move(move * Time.deltaTime * speed);
    }


    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            transform.position = Respawn.transform.position;



            GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject Cube in Cubes)
            {
                // Change color of all cubes to blue){
                Cube.GetComponent<Renderer>().material.color = Color.red; // Change color of Cube
            }


            Debug.Log("Collided with Cube!");
        }

        if (collision.gameObject.CompareTag("Finish"))
        {

            GameObject[] Cubes = GameObject.FindGameObjectsWithTag("Cube");
            foreach (GameObject Cube in Cubes)
            {
                // Change color of all cubes to blue){
                Cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(84f / 360f, 0.99f, 0.99f); // Change color of Cube
            }
            Debug.Log("Collided with Finish!");
        }


if (collision.gameObject.CompareTag("aaa"))
{
    if (shaker != null && shakePreset != null)
    {
        shaker.Shake(shakePreset);
        Debug.Log("Camera shake triggered!");
    }
    else
    {
        Debug.LogWarning("Shaker or ShakePreset not assigned!");
    }
}

    }
    
    



    // public Shaker shaker;

    // public void Shake()
    // {
    //     if (shaker != null)
    //     {
    //         ShakeInstance shakeInstance = shaker.ShakeAll(new ShakeData(0.5f, 0.5f, 0.5f, 0.5f));
    //         // Optionally, you can modify the shakeInstance properties here
    //     }
    // }



}
