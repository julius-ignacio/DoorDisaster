using UnityEngine;

public class UnCoverMechanic : MonoBehaviour
{

    public GameObject Player, Model, CoverBtn, UnCoverBtn, joystick, jumpbtn;
    public Camera CoverCamera;


    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(true); // Unhide the player
        Model.SetActive(false); // Show the cover model
        CoverCamera.gameObject.SetActive(false); // Activate the cover camera

        CoverBtn.SetActive(true); // Hide the button after clicking
        UnCoverBtn.SetActive(false); // Hide the button after clicking
        
        joystick.SetActive(true); // Show the joystick
        jumpbtn.SetActive(true); // Show the jump button
    }
}
