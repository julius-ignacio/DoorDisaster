using UnityEngine;

public class ShowBtnTrigger : MonoBehaviour
{
    public CoverMechanic cover; // assign in Inspector (same one used by the buttons)
    public GameObject button;   // Cover button GameObject

    void Start()
    {
        if (button) button.SetActive(false); // hide by default
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Only show the Cover button if NOT already covered
        if (button && cover != null && !cover.IsCovered)
            button.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (button)
            button.SetActive(false);
    }
}