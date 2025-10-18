using MilkShake;
using UnityEngine;
using Terresquall;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class MakeObjectsMove : MonoBehaviour
{
    public GameObject[] locker;

    public float chairMoveSpeed = 0.2f;
    private AudioSource audi;

    public Vector3 velocity;
    public bool isGrounded;

    private GameObject Board;
    GameObject[] chair, chair2;
    // GameObject[] books1, books2, books3;
    public ConsistentQuake consistentQuakeScript;


    void Start()
    {
        consistentQuakeScript = FindObjectOfType<ConsistentQuake>();

        Board = GameObject.FindWithTag("MovableObject");
        chair = GameObject.FindGameObjectsWithTag("Chair");
        chair2 = GameObject.FindGameObjectsWithTag("Chair2");
        // books1 = GameObject.FindGameObjectsWithTag("books1");
        // books2 = GameObject.FindGameObjectsWithTag("books2");
        // books3 = GameObject.FindGameObjectsWithTag("books3");
    }

    void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("spawnTrig"))
        {

            if (consistentQuakeScript != null && consistentQuakeScript.IsQuakeActive)
            {

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






            }

        }

        if (consistentQuakeScript != null && consistentQuakeScript.IsQuakeActive)
        {
            switch (collision.gameObject.name)
            {
                case "hallway1":
                    { locker[0].GetComponent<LockerFall>().Fall(); }
                    break;

                case "hallway2":
                    { locker[1].GetComponent<LockerFall2>().Fall(); }
                    break;

                case "hallway3":
                    {
                        locker[2].GetComponent<LockerFall>().Fall();
                        locker[3].GetComponent<LockerFall>().Fall();
                        locker[4].GetComponent<LockerFall>().Fall();
                        locker[5].GetComponent<LockerFall2>().Fall();
                    }
                    break;

                case "hallway4":
                    {
                        locker[6].GetComponent<LockerFall2>().Fall();
                        locker[7].GetComponent<LockerFall2>().Fall();
                        locker[8].GetComponent<LockerFall2>().Fall();
                    }
                    break;

                case "hallway5":
                    {
                        locker[9].GetComponent<LockerFall2>().Fall();
                        locker[10].GetComponent<LockerFall2>().Fall();
                        locker[11].GetComponent<LockerFall2>().Fall();
                        locker[12].GetComponent<LockerFall2>().Fall();
                        locker[13].GetComponent<LockerFall2>().Fall();
                        locker[20].GetComponent<LockerFall>().Fall();
                    }
                    break;


                case "hallway6":
                    {
                        locker[14].GetComponent<LockerFall2>().Fall();
                        locker[16].GetComponent<LockerFall>().Fall();
                        locker[17].GetComponent<LockerFall>().Fall();
                        locker[18].GetComponent<LockerFall>().Fall();

                    }
                    break;




                case "hallway7":
                    {
                        locker[15].GetComponent<LockerFall2>().Fall();
                        locker[19].GetComponent<LockerFall>().Fall();
                    }
                    break;

                case "hallway8":
                    {

                        locker[23].GetComponent<LockerFall2>().Fall();
                        locker[24].GetComponent<LockerFall2>().Fall();
                    }
                    break;

                case "hallway9":
                    {

                        locker[21].GetComponent<LockerFall2>().Fall();
                        locker[22].GetComponent<LockerFall2>().Fall();
                    }
                    break;


                case "hallway10":
                    {
                        locker[25].GetComponent<CeilingFall>().Fall();
                    }
                    break;

                case "hallway11":
                    {

                        locker[26].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "hallway12":
                    {

                        locker[27].GetComponent<CeilingFall>().Fall();
                    }
                    break;

                case "hallway13":
                    {

                        locker[28].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "hallway14":
                    {

                        locker[29].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "hallway15":
                    {

                        locker[30].GetComponent<CeilingFall>().Fall();
                    }
                    break;

                case "hallway16":
                    {

                        locker[31].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "MainLibRack1":
                    {

                        locker[32].GetComponent<RackFall>().Fall();
                    }
                    break;


                case "MainLibRack2":
                    {

                        locker[33].GetComponent<RackFall>().Fall();
                    }
                    break;



                case "Library2Rack":
                    {

                        locker[34].GetComponent<RackFall>().Fall();
                    }
                    break;

                case "CeilingTrigger99":
                    {

                        locker[35].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "CeilingTrigger99 (1)":
                    {

                        locker[36].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "CeilingTrigger99 (2)":
                    {

                        locker[37].GetComponent<CeilingFall>().Fall();
                    }
                    break;


                case "CeilingTrigger99 (3)":
                    {

                        locker[38].GetComponent<CeilingFall>().Fall();
                    }
                    break;   
                         case "CeilingTrigger99 (4)":
                    {

                        locker[41].GetComponent<CeilingFall>().Fall();
                    }
                    break;

                                      case "CeilingTrigger99 (5)":
                    {

                        locker[42].GetComponent<CeilingFall>().Fall();
                    }
                    break;



                case "StartLibRack":
                    {

                        locker[39].GetComponent<RackFall>().Fall();
                    }
                    break;

                        case "StartLibRack2":
                    {

                        locker[40].GetComponent<RackFall>().Fall();
                    }
                    break;

         


            }

        }





    }







}
