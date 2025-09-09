using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PanickedStateTrigger : MonoBehaviour
{
    public GameObject panickEffectUI;
    public PanicMeterScript panicMeterScript;

    public AudioSource heartbeatSFX;


    [Header("Post Processing/ Camera blur effect")]
    public Volume volume; // Assign your post-processing Volume in Inspector
    private DepthOfField dof;


    void Start()
    {
        panickEffectUI.SetActive(false);

        // Try to get DepthOfField from the volume profile
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
        if (panicMeterScript.currHealth >= 60)
        {
            panickEffectUI.SetActive(true);

            EnableBlur(true);

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

            EnableBlur(false);
        }
    }

    public void EnableBlur(bool enable)
    {
        if (dof != null)
        {
            dof.active = enable; // Enables or disables the override
        }
    }
}
