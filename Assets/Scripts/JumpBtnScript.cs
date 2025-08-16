using UnityEngine;

public class JumpBtnScript : MonoBehaviour
{
  public Movements player; // drag your movement script object here

    public void OnJumpButtonPressed()
    {
        if (player.isGrounded) // use the same grounded check
        {
            player.velocity.y = Mathf.Sqrt(player.jumpHeight * -2f * player.gravity);
        }
    }
}
