using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;  
    private Outline currentOutline;
    public Camera activeCamera; // Assign in Inspector

    void Update()
    {
        CheckInteraction();

        if (Input.GetKeyDown(KeyCode.F) && currentOutline != null)
        {
            Debug.Log("Interacted with: " + currentOutline.gameObject.name);
        }
    }

    void CheckInteraction()
    {
        // Use assigned camera instead of Camera.main
        Camera cam = activeCamera != null ? activeCamera : Camera.main;
        if (cam == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, playerReach))
        {
            Outline outline = hit.collider.GetComponent<Outline>();

            if (outline != null)
            {
                if (currentOutline != outline)
                {
                    DisableCurrentOutline();
                    currentOutline = outline;
                    currentOutline.enabled = true;
                }
                return;
            }
        }

        DisableCurrentOutline();
    }

    void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }
}
