using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Player, Model, CoverBtn, UnCoverBtn, joystick, jumpbtn;
    public Camera CoverCamera;




    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(false); // Hide the player 
        Model.SetActive(true); // Show the cover model
        CoverCamera.gameObject.SetActive(true); // Activate the cover camera
        CoverBtn.SetActive(false); // Hide the button after clicking
        UnCoverBtn.SetActive(true); // Hide the button after clicking

        joystick.SetActive(false); // Hide the joystick
        jumpbtn.SetActive(false); // Hide the jump button
    }
}
