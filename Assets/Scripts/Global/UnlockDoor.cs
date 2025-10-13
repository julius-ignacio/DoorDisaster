using EasyDoorSystem;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject colliderObject; 
    public GameObject door;
    public GetKey getkey;

    void OnTriggerEnter(Collider other)
    {
        if (getkey.isDoorLocked = false) // ✅ check global flag
        {
            colliderObject.SetActive(false);

            var doorScript = door.GetComponent<EasyDoorSystem.EasyDoor>();
            if (doorScript != null)
                doorScript.enabled = true;
        }
    }
}
