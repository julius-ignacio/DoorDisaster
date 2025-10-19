using MilkShake;
using UnityEngine;

public class PlayIsabellaLullabyOST_StopQuake : MonoBehaviour
{
    public ConsistentQuake consistentQuake;
    public Shaker shake;
    public PanicMeterScript panicMeterScript;

    
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Audio switching logic (this part always runs)
        AudioClip temp = AudioManager.Instance.Clips[2];
        AudioManager.Instance.Clips[2] = AudioManager.Instance.Clips[1];
        AudioManager.Instance.Clips[1] = temp;

        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[20]);
        AudioManager.Instance.audLoop.volume = 0.5f;

        // Safely handle quake + shake
        if (shake != null)
            shake.enabled = false;

        if (consistentQuake != null)
        {
            consistentQuake.enabled = false;
            consistentQuake.PauseQuakes();
        }

               if (panicMeterScript != null)
        {
            panicMeterScript.currHealth = 0;
        }
    }
}
