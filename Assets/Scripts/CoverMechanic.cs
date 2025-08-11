using UnityEngine;

public class CoverMechanic : MonoBehaviour
{

    public GameObject Player;
    public Camera CoverCamera;


 public void OnButtonClick()
    {
        Debug.Log("Button was clicked!");
        Player.SetActive(false); // Hide the player
        CoverCamera.gameObject.SetActive(true); // Activate the cover camera
    }
}
