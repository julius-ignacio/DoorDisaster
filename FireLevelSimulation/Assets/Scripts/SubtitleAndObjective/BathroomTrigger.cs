using UnityEngine;

public class BathroomTrigger : MonoBehaviour
{
    public SubtitleManager subtitleManager;
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            subtitleManager.ShowCustomMessage(
                "It's hard to breathe with all this smoke! I need a wet towel to cover my face!",
                3f,
                () => subtitleManager.ShowObjective("Find a wet towel in the bathroom")

            );
        }
    }
}
