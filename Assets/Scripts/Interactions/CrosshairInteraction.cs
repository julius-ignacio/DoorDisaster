using UnityEngine;

public class CrosshairInteraction : MonoBehaviour
{
    public float interactDistance = 3f;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Check if the object has the "Interactable" tag
            if (hit.collider.CompareTag("Interactable"))
            {
                // Optional: show UI feedback like "Press E to interact"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Interacted with " + hit.collider.name);
                    // Add your interaction logic here
                    // Example: destroy the object
                    // Destroy(hit.collider.gameObject);
                }
            }
        }
    }
}
