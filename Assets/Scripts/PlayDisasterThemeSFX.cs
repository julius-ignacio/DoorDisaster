using UnityEngine;

public class PlayDisasterThemeSFX : MonoBehaviour
{
    public int selectedTheme;
    public GameObject stopLoopSignal;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop any currently playing loop before starting a new one
        AudioManager.Instance.audLoop.Stop();

        // Play the selected disaster theme loop
        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[selectedTheme]);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop playing when the player leaves the area
        AudioManager.Instance.audLoop.Stop();
    }
}
