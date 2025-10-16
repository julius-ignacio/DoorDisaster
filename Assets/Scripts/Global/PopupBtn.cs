using UnityEngine;

public class PopupBtn : MonoBehaviour
{
    public GameObject popupButton; // Assign the UI Button GameObject here
    public int trialId; // 0 = Earth, 1 = Fire, 2 = Water, etc.

    void Start()
    {
        popupButton.SetActive(false); // Hide by default
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupButton.SetActive(true); // Show button when player is near
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupButton.SetActive(false); // Hide button when player leaves
        }
    }
}
