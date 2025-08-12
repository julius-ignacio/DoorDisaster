using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Player, Model;
    public Camera CoverCamera;

    void Start()
    {
        Player.SetActive(true); // Ensure player is active at start
        Model.SetActive(false); // Ensure cover model is inactive at start
        CoverCamera.gameObject.SetActive(false); // Ensure cover camera is inactive at start
    }


    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(false); // Hide the player
        Model.SetActive(true); // Show the cover model
        CoverCamera.gameObject.SetActive(true); // Activate the cover camera
    }
}
