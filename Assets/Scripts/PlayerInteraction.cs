using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float playerReach = 3f;  
    private Outline currentOutline;

    void Update()
    {
        CheckInteraction();

        // Example: Press F to interact
        if (Input.GetKeyDown(KeyCode.F) && currentOutline != null)
        {
            Debug.Log("Interacted with: " + currentOutline.gameObject.name);
        }
    }

    void CheckInteraction()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
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
                    currentOutline.enabled = true; // Turn on highlight
                }
                return;
            }
        }

        // If looking at nothing / non-outline
        DisableCurrentOutline();
    }

    void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false; // Turn off highlight
            currentOutline = null;
        }
    }
}
