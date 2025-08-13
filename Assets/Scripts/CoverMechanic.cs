using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Player, Model;
    public Camera CoverCamera;




    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(false); // Hide the player
        Model.SetActive(true); // Show the cover model
        CoverCamera.gameObject.SetActive(true); // Activate the cover camera
    }
}
