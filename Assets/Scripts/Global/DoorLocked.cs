using EasyDoorSystem;
using UnityEngine;

public class DoorLocked : MonoBehaviour
{
    public GameObject colliderObject, getKeyBtn, getKeyTrigger; 
    public GameObject door, doorlockPrompt;
    public GetKey getkey;

    void Start()
    {
        colliderObject.SetActive(true);
        doorlockPrompt.SetActive(true);
    }


    void OnTriggerEnter(Collider other)
    {
        if (getkey.isDoorLocked == false) // ✅ check global flag
        {
            colliderObject.SetActive(false);

            doorlockPrompt.SetActive(false);

            // var doorScript = door.GetComponent<EasyDoorSystem.EasyDoor>();
            // doorScript.enabled = true;

            door.GetComponent<EasyDoorSystem.EasyDoor>().enabled = true;
        }
        
                else // ✅ check global flag
        {
            colliderObject.SetActive(true);
            doorlockPrompt.SetActive(true);
                door.GetComponent <EasyDoorSystem.EasyDoor>().enabled = false;
        }
    }
}
