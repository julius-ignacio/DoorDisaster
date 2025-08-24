using EasyDoorSystem;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject colliderObject; 
    public GameObject door;

    void OnTriggerEnter(Collider other)
    {
        if (PickUpScript.hasKeycard) // ✅ check global flag
        {
            colliderObject.SetActive(false);

            var doorScript = door.GetComponent<EasyDoorSystem.EasyDoor>();
            if (doorScript != null)
                doorScript.enabled = true;
        }
    }
}
