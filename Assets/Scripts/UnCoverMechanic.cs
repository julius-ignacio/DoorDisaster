using UnityEngine;

public class UnCoverMechanic : MonoBehaviour
{

    public GameObject Player;
    public Camera CoverCamera;


 public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(true); // Unhide the player
        CoverCamera.gameObject.SetActive(false); // Activate the cover camera
    }
}
