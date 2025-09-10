using UnityEngine;

public class PlayDisasterThemeSFX : MonoBehaviour
{
    public AudioSource disasterThemeSFX; // Assign the AudioSource component in the Inspector

    void OnTriggerEnter(Collider other)
    {
        disasterThemeSFX.Play();
    }

     void OnTriggerExit(Collider other)
    {
        disasterThemeSFX.Stop();
    }
}
