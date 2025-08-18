using EasyDoorSystem;
using UnityEngine;

public class DoorLocked : MonoBehaviour
{
    public GameObject colliderObject; 
    public GameObject door;

    void OnTriggerEnter(Collider other)
    {
        if (!PickUpScript.hasKeycard) // ✅ check global flag
        {
            colliderObject.SetActive(true);

            var doorScript = door.GetComponent<EasyDoorSystem.EasyDoor>();
                doorScript.enabled = false;
        }
    }
}
