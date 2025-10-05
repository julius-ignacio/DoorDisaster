using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Add your custom logic here, e.g., pick up, open door, etc.
    }
}
