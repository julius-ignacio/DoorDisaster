using UnityEngine;

public class PopupBtn : MonoBehaviour
{
    public GameObject popupButton; // Assign the UI Button GameObject here


    void Start()
    {
                    popupButton.SetActive(false); // Hide the button
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupButton.SetActive(true); // Show the button
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            popupButton.SetActive(false); // Hide the button
        }
    }
}
