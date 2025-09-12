using UnityEngine;

public class PlayDisasterThemeSFX : MonoBehaviour
{
    public AudioManager aud; // Assign the AudioSource component in the Inspector
    public int selectedTheme;

    void OnTriggerEnter(Collider other)
    {
       if (!other.CompareTag("Player")) return;

        // Play the selected disaster theme based on the selectedTheme variable
        aud.PlayLoop(aud.Clips[selectedTheme]); // play looped theme
    }

    void OnTriggerExit(Collider other)
    {
        aud.audLoop.Stop();
    }
}
