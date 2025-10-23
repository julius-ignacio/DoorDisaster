using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class PlayDisasterThemeSFX : MonoBehaviour
{
    public int selectedTheme;
    public GameObject stopLoopSignal;
    public int trialIndex;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop any currently playing loop before starting a new one
        AudioManager.Instance.audLoop.Stop();
        AudioManager.Instance.audLoopSecondary.Stop();

        // Play the selected disaster theme loop

        switch (trialIndex)
        {
            case 0: // Earth
                if (DataManager.Instance.playerData.isWaterFinished)
                    SFX();
                else
                    PlayLockedTrialSFX();
                break;

            case 1: // Water
                if (DataManager.Instance.playerData.isFireFinished)
                    SFX();
                else
                    PlayLockedTrialSFX();
                break;

            case 2: // Water
                SFX();
                break;
        }

    }

    private void PlayLockedTrialSFX()
    {
        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[33]); //whisper
        AudioManager.Instance.PlayLoopSecondary(AudioManager.Instance.Clips[34]); //wind
    }

    private void SFX()
    {
        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[selectedTheme]);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Stop playing when the player leaves the area
        AudioManager.Instance.audLoop.Stop();
        AudioManager.Instance.audLoopSecondary.Stop();
    }
}

