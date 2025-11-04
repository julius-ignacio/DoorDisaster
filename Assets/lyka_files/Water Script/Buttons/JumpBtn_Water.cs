using UnityEngine;

public class JumpBtn_Water : MonoBehaviour
{
    public PlayerController_Water player;

    public void OnJumpButtonPressed()
    {
        if (player != null && player.canMove && player.IsGrounded)
        {
            player.Jump(); // Calls the controller's Jump method
        }
    }
}
