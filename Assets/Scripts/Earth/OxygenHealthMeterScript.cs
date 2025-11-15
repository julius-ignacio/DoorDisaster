using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OxygenHealthMeterScript : MonoBehaviour
{
    [Header("UI")]
    public Slider oxygenHealthSlider;
    public Image fill;

    [Header("Health values")]
    public int maxHealth = 100;
    public float currHealth = 100f;

    [Header("Drain when drowning")]
    [Tooltip("Health loss per second while player is drowning (oxygen at 0).")]
    public float healthDrainPerSecond = 10f;

    [Header("Audio/FX")]
    public GameObject injuryEffectUI; // optional overlay
    private bool heartbeatPlaying = false; // heartbeat loop state

    // [Header("Optional Post Processing")]
    // public Volume volume;
    // private DepthOfField dof;

    [HideInInspector] public bool Drowning = false;

    void Start()
    {
        if (maxHealth <= 0) maxHealth = 100;
        currHealth = Mathf.Clamp(currHealth, 0f, maxHealth);

        if (oxygenHealthSlider != null)
        {
            oxygenHealthSlider.minValue = 0f;
            oxygenHealthSlider.maxValue = maxHealth;
            oxygenHealthSlider.value = currHealth;
        }

        // if (volume != null && volume.profile != null)
        // {
        //     volume.profile.TryGet(out dof);
        // }
    }

    void Update()
    {
        // Drain only while drowning
        if (Drowning && currHealth > 0f)
        {
            currHealth = Mathf.Max(0f, currHealth - healthDrainPerSecond * Time.deltaTime);
        }

        // Update UI
        if (oxygenHealthSlider != null)
            oxygenHealthSlider.value = currHealth;

        // Heartbeat loop when low health
        if (currHealth <= 50f)
        {
            if (!heartbeatPlaying && AudioManager.Instance != null)
            {
                // Ensure we have a valid clip index; replace 17 if needed
                if (AudioManager.Instance.Clips != null &&
                    AudioManager.Instance.Clips.Length > 17 &&
                    AudioManager.Instance.Clips[17] != null)
                {
                    AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[17]);
                    heartbeatPlaying = true;
                }
            }
        }
        else
        {
            if (heartbeatPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.StopLoop();
                heartbeatPlaying = false;
            }
        }

        // Optional: show injury overlay while drowning or low health
        if (injuryEffectUI != null)
            injuryEffectUI.SetActive(Drowning || currHealth <= 50f);
    }
}