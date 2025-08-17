using EasyDoorSystem;
using UnityEngine;

public class UnlockDoor : MonoBehaviour
{
    public GameObject colliderObject; // Assign the collider object in the Inspector
    public GameObject door; // Assign the door GameObject in the Inspector\

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Only trigger when Player collides
        {
            colliderObject.SetActive(false); // Disable collider (unlocks path)

            // Example: enable door movement script if it’s disabled
            var doorScript = door.GetComponent<EasyDoorSystem.EasyDoor>();
            if (doorScript != null)
                doorScript.enabled = true;
                

                
        }
    }
}
