// using MilkShake;
// using UnityEngine;
// using Terresquall;
// using System.Collections;

// public class ShortEQ : MonoBehaviour
// {

//     public Shaker shaker;
//     public ShakePreset shakePreset;
//     public PanicMeterScript panicMeterScript; // Reference to the PanicMeterScript



//     public float chairMoveSpeed = 1f; 

//     private AudioManager aud;

//     public Vector3 velocity;
//     public bool isGrounded;

//     private GameObject Board, LowEQTrigger;
//     GameObject[] chair, chair2;


//     void Start()
//     {
//         Board = GameObject.FindWithTag("MovableObject");
//         chair = GameObject.FindGameObjectsWithTag("Chair");
//         chair2 = GameObject.FindGameObjectsWithTag("Chair2");
//     }


//     void OnTriggerEnter(Collider collision)
//     {

//         if (collision.gameObject.CompareTag("Player"))
//         {

//             aud.Clips; // Play sound effect on collision
//             Board.isStatic = false;

//             if (shaker != null || shakePreset != null)
//             {
//                 shaker.Shake(shakePreset);
//                 Debug.Log("Camera shake triggered!");
//             }
//             else
//             {
//                 Debug.LogWarning("Shaker or ShakePreset not assigned!");

//             }


//             // Delay the fall of MovableObjects
//             StartCoroutine(DelayFall(4f));

//             // Move 'chair' objects towards random positions
//             foreach (GameObject obj in chair)
//             {
//                 Vector3 randomTarget = new Vector3(
//                     Random.Range(-1f, 1f),
//                     Random.Range(-1f, 1f),
//                     Random.Range(-1f, 1f)
//                 );
//                 obj.transform.position = Vector3.MoveTowards(obj.transform.position, randomTarget, chairMoveSpeed * Time.deltaTime);
//             }

//             // Move 'chair2' objects away from random positions (opposite direction)
//             foreach (GameObject obj in chair2)
//             {
//                 Vector3 randomTarget = new Vector3(
//                     Random.Range(-1f, 1f),
//                     Random.Range(-1f, 1f),
//                     Random.Range(-1f, 1f)
//                 );
//                 // Calculate direction away from randomTarget
//                 Vector3 directionAway = (obj.transform.position - randomTarget).normalized;
//                 obj.transform.position += directionAway * chairMoveSpeed * Time.deltaTime;
//             }


//             LowEQTrigger.SetActive(false); // Deactivate the LowEQTrigger

//         }



//     }

//    IEnumerator DelayFall(float delay)
//     {
//         yield return new WaitForSeconds(delay);

//         // Find all MovableObjects and let them fall
//         GameObject[] movableObjects = GameObject.FindGameObjectsWithTag("MovableObject");
//         foreach (GameObject obj in movableObjects)
//         {
//             Rigidbody rb = obj.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 rb.isKinematic = false; 
//                 rb.useGravity = true;   
//             }
//         }
//     }


// }
