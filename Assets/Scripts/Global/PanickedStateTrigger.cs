using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PanickedStateTrigger : MonoBehaviour
{
    public GameObject panickEffectUI;
    public PanicMeterScript panicMeterScript;

    public AudioManager aud;

    [Header("Post Processing/ Camera blur effect")]
    public Volume volume; 
    private DepthOfField dof;

    private bool heartbeatPlaying = false; // track loop state

    void Start()
    {
        panickEffectUI.SetActive(false);

        if (volume.profile.TryGet(out dof))
        {
            Debug.Log("Depth of Field found!");
        }
        else
        {
            Debug.LogWarning("No Depth of Field override found in this Volume profile!");
        }
    }

    void Update()
    {
        if (panicMeterScript.currHealth >= 75)
        {
            // PANIC MAX — blur and heartbeat
            panickEffectUI.SetActive(true);
            EnableBlur(true);

            if (!heartbeatPlaying)
            {
                aud.PlayLoop(aud.Clips[8]); // heartbeat loop
                heartbeatPlaying = true;
            }
        }
        else if (panicMeterScript.currHealth >= 60)
        {
            // Mild panic — only UI
            panickEffectUI.SetActive(true);
            EnableBlur(false);

            if (heartbeatPlaying)
            {
                aud.StopLoop();
                heartbeatPlaying = false;
            }
        }
        else
        {
            // Calm
            panickEffectUI.SetActive(false);
            EnableBlur(false);

            if (heartbeatPlaying)
            {
                aud.StopLoop();
                heartbeatPlaying = false;
            }
        }
    }

    public void EnableBlur(bool enable)
    {
        if (dof != null)
        {
            dof.active = enable;
        }
    }
}
