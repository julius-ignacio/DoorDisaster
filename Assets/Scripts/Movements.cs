using MilkShake;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movements : MonoBehaviour
{

    public Shaker shaker, shaker3rdPerson;
    public ShakePreset shakePreset;



    public float speed = 20f;
    public float gravity = -9.81f;
    public float jumpHeight = 7f;

    private AudioSource audi;
    private Animator Animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private GameObject Cube, Respawn, Finish, Board, FirstPersonCam, ThirdPersonCam;
    GameObject[] chair, chair2;

    LockerScript ls;


    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();

        Respawn = GameObject.FindWithTag("Respawn");

        Board = GameObject.FindWithTag("MovableObject");

        Finish = GameObject.FindWithTag("Finish");

        FirstPersonCam = GameObject.FindWithTag("FirstPersonCamera");

        ThirdPersonCam = GameObject.FindWithTag("ThirdPersonCamera");


        chair = GameObject.FindGameObjectsWithTag("Chair");
        chair2 = GameObject.FindGameObjectsWithTag("Chair2");

        audi = GetComponent<AudioSource>();


        ls = FindObjectOfType<LockerScript>();

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

        // // Get input (Keyboard or Virtual Joystick)
        float x = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float z = Input.GetAxis("Vertical");   // W/S or Up/Down


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


    if (move != Vector3.zero)
{
    Animator.SetBool("IsMoving", true);
}
else
{
    Animator.SetBool("IsMoving", false);
}

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
            audi.Play(); // Play sound effect on collision
            Board.isStatic = false;
            ls.lockersound.Play(); // Play locker sound effect on collision

            // Find all MovableObjectsz
            GameObject[] movableObjects = GameObject.FindGameObjectsWithTag("MovableObject");

            foreach (GameObject obj in movableObjects)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false; // Enable movement
                    rb.useGravity = true;   // Make sure gravity is applied
                }
            }



            if (shaker != null || shaker3rdPerson != null && shakePreset != null)
            {
                shaker.Shake(shakePreset);
                shaker3rdPerson.Shake(shakePreset);
                Debug.Log("Camera shake triggered!");
            }
            else
            {
                Debug.LogWarning("Shaker or ShakePreset not assigned!");

            }




            // Move 'chair' objects towards random positions
            foreach (GameObject obj in chair)
            {
                Vector3 randomTarget = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, randomTarget, 0.1f * Time.deltaTime);
            }

            // Move 'chair2' objects away from random positions (opposite direction)
            foreach (GameObject obj in chair2)
            {
                Vector3 randomTarget = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
                // Calculate direction away from randomTarget
                Vector3 directionAway = (obj.transform.position - randomTarget).normalized;
                obj.transform.position += directionAway * 0.1f * Time.deltaTime;
            }




            //CameraSwitcher
             FirstPersonCam.SetActive(false);
         ThirdPersonCam.SetActive(true);







        }
        else
        {
              FirstPersonCam.SetActive(true);
              ThirdPersonCam.SetActive(false);
        }



    }







}
