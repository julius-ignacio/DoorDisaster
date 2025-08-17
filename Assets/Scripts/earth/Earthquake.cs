using MilkShake;
using UnityEngine;
using Terresquall;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Earthquake : MonoBehaviour
{

    public Shaker shaker;
    public ShakePreset shakePreset;
    public PanicMeterScript panicMeterScript; // Reference to the PanicMeterScript



    public float chairMoveSpeed = 1f; 
    public GameObject locker;

    private AudioSource audi;

    public Vector3 velocity;
    public bool isGrounded;

    private GameObject Board;
    GameObject[] chair, chair2;


    void Start()
    {
        Board = GameObject.FindWithTag("MovableObject");
        chair = GameObject.FindGameObjectsWithTag("Chair");
        chair2 = GameObject.FindGameObjectsWithTag("Chair2");
        audi = GetComponent<AudioSource>();
    }


    void OnControllerColliderHit(ControllerColliderHit collision)
    {

        if (collision.gameObject.CompareTag("EqStart")) {
            if (shaker != null || shakePreset != null)
            {
                shaker.Shake(shakePreset);
                Debug.Log("Camera shake triggered!");
            }
            else
            {
                Debug.LogWarning("Shaker or ShakePreset not assigned!");

            }
        }






        if (collision.gameObject.CompareTag("aaa"))
        {

            locker.GetComponent<LockerFall>().Fall(); // Call the Fall method on LockerFall script

            audi.Play(); // Play sound effect on collision
            Board.isStatic = false;

            // Find all MovableObjectsz
            // GameObject[] movableObjects = GameObject.FindGameObjectsWithTag("MovableObject");

            // foreach (GameObject obj in movableObjects)
            // {
            //     Rigidbody rb = obj.GetComponent<Rigidbody>();
            //     if (rb != null)
            //     {
            //         rb.isKinematic = false; // Enable movement
            //         rb.useGravity = true;   // Make sure gravity is applied
            //     }
            // }



            if (shaker != null || shakePreset != null)
            {
                shaker.Shake(shakePreset);
                Debug.Log("Camera shake triggered!");
            }
            else
            {
                Debug.LogWarning("Shaker or ShakePreset not assigned!");

            }


            // Delay the fall of MovableObjects
            StartCoroutine(DelayFall(4f));



            // Move 'chair' objects towards random positions
            foreach (GameObject obj in chair)
            {
                Vector3 randomTarget = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, randomTarget, chairMoveSpeed * Time.deltaTime);
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
                obj.transform.position += directionAway * chairMoveSpeed * Time.deltaTime;
            }


            if (panicMeterScript != null)
            {
                panicMeterScript.currHealth += 0.01f; // Increase by 10 (or any value you want)
                panicMeterScript.currHealth = Mathf.Clamp(panicMeterScript.currHealth, 0, panicMeterScript.maxHealth);
            }



        }



    }

   IEnumerator DelayFall(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Find all MovableObjects and let them fall
        GameObject[] movableObjects = GameObject.FindGameObjectsWithTag("MovableObject");
        foreach (GameObject obj in movableObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; 
                rb.useGravity = true;   
            }
        }
    }


}
