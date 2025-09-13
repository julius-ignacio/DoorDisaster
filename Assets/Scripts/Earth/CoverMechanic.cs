using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Model, CoverBtn, UnCoverBtn, joystick, jumpbtn, RestrictionWall;
    public Camera CoverCamera, playerCamera;
    public AudioManager footsteps_disable;
    
    public Movements PlayerMovements;

    void Start()
    {
    }


    public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Model.SetActive(true); // Show the cover model
        CoverCamera.enabled = true; // Enable the cover camera
        playerCamera.enabled = false; // Enable the cover camera
        CoverBtn.SetActive(false); // Hide the button after clicking
        UnCoverBtn.SetActive(true);//show uncoverbutton

            // Disable footsteps
    PlayerMovements.footstepsEnabled = false;
    footsteps_disable.audClip.Stop(); 

        joystick.SetActive(false); // Hide the joystick
        jumpbtn.SetActive(false); // Hide the jump button


        PlayerMovements.speed = 0f;
        PlayerMovements.jumpHeight = 0f;
    }


    public void OnButtonClick2()
    {
        Debug.Log("Button was clicked2!");
        RestrictionWall.SetActive(false); // Hide the restriction wall
    }
}
