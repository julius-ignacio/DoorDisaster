using UnityEngine;

public class BlockedPathTrigger : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager2 subtitleManager;

    private bool messageShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !messageShown)
        {
            messageShown = true;

            // Show the message only once
            subtitleManager.ShowCustomMessage(
                "Let's not go there... the fire already spread out!",
                2.5f
            );
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Reset so the player can see it again if they try a second time
            messageShown = false;
        }
    }
}
