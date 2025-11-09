using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PanicMeterScript2 : MonoBehaviour
{
    [Header("UI References")]
    public Slider panicMeterSlider;
    public GameObject panickEffectUI;

    [Header("Panic Meter Settings")]
    public int maxHealth = 100; // max calm
    public float currHealth;    // current health (goes from 100 → 0)

    private bool heartbeatPlaying = false; // track loop state

    [Header("Post Processing / Camera Blur Effect")]
    public Volume volume;
    private DepthOfField dof;

    void Start()
    {
        // Start full (calm)
        currHealth = maxHealth;

        // Hide panic UI initially
        if (panickEffectUI != null)
            panickEffectUI.SetActive(false);

        // Find Depth of Field effect
        if (volume != null && volume.profile.TryGet(out dof))
            Debug.Log("Depth of Field found!");
        else
            Debug.LogWarning("No Depth of Field override found in this Volume profile!");
    }

    void Update()
    {
        // Clamp and update slider
        currHealth = Mathf.Clamp(currHealth, 0, maxHealth);
        panicMeterSlider.maxValue = maxHealth;
        panicMeterSlider.value = currHealth;

        // --- CALM (80-100) ---
        if (currHealth >= 80f)
        {
            CalmState();
        }
        // --- MILD PANIC (50-79) ---
        else if (currHealth >= 50f)
        {
            MildPanicState();
        }
        // --- FULL PANIC (0-49) ---
        else
        {
            FullPanicState();
        }
    }

    private void CalmState()
    {
        if (panickEffectUI != null)
            panickEffectUI.SetActive(false);
        EnableBlur(false);
        StopHeartbeat();
    }

    private void MildPanicState()
    {
        if (panickEffectUI != null)
            panickEffectUI.SetActive(true);
        EnableBlur(false);
        PlayHeartbeat();
    }

    private void FullPanicState()
    {
        if (panickEffectUI != null)
            panickEffectUI.SetActive(true);
        EnableBlur(true);
        PlayHeartbeat();
    }

    private void PlayHeartbeat()
    {
        if (!heartbeatPlaying)
        {
            AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[17]);
            heartbeatPlaying = true;
        }
    }

    private void StopHeartbeat()
    {
        if (heartbeatPlaying)
        {
            AudioManager.Instance.StopLoop();
            heartbeatPlaying = false;
        }
    }

    public void EnableBlur(bool enable)
    {
        if (dof != null)
            dof.active = enable;
    }
}
