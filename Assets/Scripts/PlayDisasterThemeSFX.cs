using UnityEngine;

public class PlayDisasterThemeSFX : MonoBehaviour
{
    public int selectedTheme;
    public GameObject stopLoopSignal;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Play the selected disaster theme based on the selectedTheme variable
        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[selectedTheme]); // play looped theme

        if (stopLoopSignal == null) return;
         AudioManager.Instance.audLoop.Stop();
         
    }

    void OnTriggerExit(Collider other)
    {
         AudioManager.Instance.audLoop.Stop();
    }
}
