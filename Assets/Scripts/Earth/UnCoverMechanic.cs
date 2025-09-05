using UnityEngine;

public class UnCoverMechanic : MonoBehaviour
{

    public GameObject Model, CoverBtn, UnCoverBtn, joystick, jumpbtn;
    public Camera CoverCamera, playerCamera;
    public GameObject footsteps_enable;

    public Movements PlayerMovements;
    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Model.SetActive(false); // Show the cover model
        CoverCamera.enabled = false; // Disable the cover camera
        playerCamera.enabled = true; // Enable the player camera
        //PlayerMovements.GetComponent<Movements>().enabled = true; // Disable player movement

        CoverBtn.SetActive(true); // Hide the button after clicking
        UnCoverBtn.SetActive(false); // Hide the button after clicking

        joystick.SetActive(true); // Show the joystick
        jumpbtn.SetActive(true); // Show the jump button

        footsteps_enable.SetActive(true);



        PlayerMovements.speed = 3f;
        PlayerMovements.jumpHeight = 1f;
    }
}
