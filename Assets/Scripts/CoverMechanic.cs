using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Player, Model, CoverBtn, UnCoverBtn, joystick, jumpbtn, RestrictionWall;
    public Camera CoverCamera, playerCamera;




    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Model.SetActive(true); // Show the cover model
        CoverCamera.enabled = true; // Enable the cover camera
        playerCamera.enabled = false; // Enable the cover camera
        Player.GetComponent<CharacterController>().enabled = false; // Disable player movement
        CoverBtn.SetActive(false); // Hide the button after clicking
        UnCoverBtn.SetActive(true);//show uncoverbutton

        joystick.SetActive(false); // Hide the joystick
        jumpbtn.SetActive(false); // Hide the jump button
    }


    public void OnButtonClick2()
    {
        Debug.Log("Button was clicked2!");
        RestrictionWall.SetActive(false); // Hide the restriction wall
    }
}
