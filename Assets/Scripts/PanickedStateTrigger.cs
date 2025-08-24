using UnityEngine;

public class PanickedStateTrigger : MonoBehaviour
{
    public GameObject panickEffectUI;
    public PanicMeterScript panicMeterScript;

    public AudioSource heartbeatSFX;

    void Update()
    {
        if (panicMeterScript.currHealth >= 60)
        {
            panickEffectUI.SetActive(true);

            if (!heartbeatSFX.isPlaying) // only start once
            {
                heartbeatSFX.Play();
            }
        }
        else
        {
            if (heartbeatSFX.isPlaying)
            {
                heartbeatSFX.Stop();
            }
            panickEffectUI.SetActive(false);
        }
    }
}
