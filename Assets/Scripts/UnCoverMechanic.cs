using UnityEngine;

public class UnCoverMechanic : MonoBehaviour
{

    public GameObject Player, Model, CoverBtn, UnCoverBtn, joystick, jumpbtn;
    public Camera CoverCamera, playerCamera;


    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Model.SetActive(false); // Show the cover model
        CoverCamera.enabled = false; // Disable the cover camera
        playerCamera.enabled = true; // Enable the player camera
        Player.GetComponent<CharacterController>().enabled = true; // Enable player movement

        CoverBtn.SetActive(true); // Hide the button after clicking
        UnCoverBtn.SetActive(false); // Hide the button after clicking
        
        joystick.SetActive(true); // Show the joystick
        jumpbtn.SetActive(true); // Show the jump button
    }
}
