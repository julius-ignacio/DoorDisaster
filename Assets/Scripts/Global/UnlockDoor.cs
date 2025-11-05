using EasyDoorSystem;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject colliderObject; 
    public GameObject door;
    public GetKey getkey;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (getkey != null && getkey.isDoorLocked == false) // fixed comparison
        {
            if (colliderObject) colliderObject.SetActive(false);

            var doorScript = door ? door.GetComponent<EasyDoor>() : null;
            if (doorScript) doorScript.enabled = true;
        }
    }
}