using UnityEngine;

public class CoverMechanic : MonoBehaviour
{
    public GameObject Model, CoverBtn, UnCoverBtn, joystick, jumpbtn, RestrictionWall;
    public Camera CoverCamera, playerCamera;
    public Movements PlayerMovements;

    public bool IsCovered { get; private set; } = false;

    void Awake()
    {
        // Default: not covered and cover button hidden (your trigger will show it)
        IsCovered = false;

        if (Model) Model.SetActive(false);
        if (CoverCamera) CoverCamera.enabled = false;
        if (playerCamera) playerCamera.enabled = true;

        if (UnCoverBtn) UnCoverBtn.SetActive(false);
        if (CoverBtn) CoverBtn.SetActive(false); // trigger plane will show when player is near
    }

    // Single source of truth to apply covered/uncovered UI + movement
    public void ApplyCoveredState(bool covered)
    {
        IsCovered = covered;

        if (covered)
        {
            if (Model) Model.SetActive(true);
            if (CoverCamera) CoverCamera.enabled = true;
            if (playerCamera) playerCamera.enabled = false;

            if (CoverBtn) CoverBtn.SetActive(false);
            if (UnCoverBtn) UnCoverBtn.SetActive(true);

            if (PlayerMovements)
            {
                PlayerMovements.footstepsEnabled = false;
                PlayerMovements.speed = 0f;
                PlayerMovements.jumpHeight = 0f;
            }

            if (joystick) joystick.SetActive(false);
            if (jumpbtn) jumpbtn.SetActive(false);

            if (AudioManager.Instance) AudioManager.Instance.audClip.Stop();
        }
        else
        {
            if (Model) Model.SetActive(false);
            if (CoverCamera) CoverCamera.enabled = false;
            if (playerCamera) playerCamera.enabled = true;

            // Important: do NOT force CoverBtn visible here.
            // Your trigger zone will show it when the player is inside the area.
            if (UnCoverBtn) UnCoverBtn.SetActive(false);

            if (PlayerMovements)
            {
                PlayerMovements.footstepsEnabled = true;
                PlayerMovements.speed = 3f;
                PlayerMovements.jumpHeight = 1f;
            }

            if (joystick) joystick.SetActive(true);
            if (jumpbtn) jumpbtn.SetActive(true);
        }
    }

    // UI button: Cover
    public void OnButtonClick()
    {
        ApplyCoveredState(true);
    }

    // Optional: remove a restriction while covered
    public void OnButtonClick2()
    {
        if (RestrictionWall) RestrictionWall.SetActive(false);
    }
}