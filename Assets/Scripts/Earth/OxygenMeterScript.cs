using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OxygenMeterScript : MonoBehaviour
{
    [Header("UI")]
    public Slider oxygenMeterSlider;
    public Image fill;

    [Header("Oxygen values")]
    public int maxHealth = 100;       // maximum oxygen
    public float currHealth = 100f;   // current oxygen

    [Header("Drain when underwater")]
    [Tooltip("Oxygen loss per second while head is underwater.")]
    public float headDrainPerSecond = 5f;

    [Tooltip("Optional extra drain when only knees are underwater (set 0 if not used).")]
    public float kneeDrainPerSecond = 0f;

    // These are set via RisingWater events
    private bool isHeadUnderwater = false;
    private bool isKneeUnderwater = false;

    [Header("FX")]
    public GameObject panickEffectUI;



    [Header("Post Processing/ Camera blur effect")]
    public Volume volume;
    private DepthOfField dof;

    [Header("References")]
    public OxygenHealthMeterScript oxygenHealthMeterScript;

    void Start()
    {
        if (maxHealth <= 0) maxHealth = 100;
        currHealth = Mathf.Clamp(currHealth, 0f, maxHealth);

        if (oxygenMeterSlider != null)
        {
            oxygenMeterSlider.minValue = 0f;
            oxygenMeterSlider.maxValue = maxHealth;
            oxygenMeterSlider.value = currHealth;
        }

        // Set initial drowning state based on starting oxygen
        if (oxygenHealthMeterScript != null)
        {
            oxygenHealthMeterScript.Drowning = (currHealth <= 0f);
        }

        if (panickEffectUI != null)
            panickEffectUI.SetActive(false);

        if (volume != null && volume.profile != null && volume.profile.TryGet(out dof))
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
        // Apply oxygen drain when underwater
        float drain = 0f;
        if (isHeadUnderwater) drain += headDrainPerSecond;
        if (!isHeadUnderwater && isKneeUnderwater) drain += kneeDrainPerSecond; // knees-only case

        if (drain > 0f && currHealth > 0f)
        {
            currHealth = Mathf.Max(0f, currHealth - drain * Time.deltaTime);
        }

        // Toggle blur on head underwater
        EnableBlur(isHeadUnderwater);

        // Update slider
        if (oxygenMeterSlider != null)
        {
            oxygenMeterSlider.maxValue = maxHealth;
            oxygenMeterSlider.value = currHealth;
        }

        // Determine panic color/effect by missing oxygen percentage
        float panicPct = (maxHealth > 0)
            ? Mathf.Clamp01((maxHealth - currHealth) / maxHealth) * 100f
            : 0f;

        if (panicPct >= 80f)
        {
            if (fill != null) fill.color = new Color32(154, 15, 15, 255); // deep red
            if (panickEffectUI != null) panickEffectUI.SetActive(true);
        }
        else if (panicPct >= 70f)
        {
            if (fill != null) fill.color = new Color32(241, 75, 65, 255); // red
            if (panickEffectUI != null) panickEffectUI.SetActive(true);
        }
        else if (panicPct >= 50f)
        {
            if (fill != null) fill.color = new Color32(241, 157, 84, 255); // orange
            if (panickEffectUI != null) panickEffectUI.SetActive(false);
        }
        else
        {
            if (fill != null) fill.color = new Color32(66, 244, 184, 255); // teal/green
            if (panickEffectUI != null) panickEffectUI.SetActive(false);
        }

        // If oxygen is 0, start drowning (health drain). Else, stop drowning.
        if (oxygenHealthMeterScript != null)
        {
            oxygenHealthMeterScript.Drowning = (currHealth <= 0f);
        }

    }


    public void EnableBlur(bool enable)
    {
        if (dof != null)
        {
            dof.active = enable;
        }
    }

    // Call these from your water detection system (e.g., RisingWater events)
    public void SetHeadUnderwater(bool underwater)
    {
        isHeadUnderwater = underwater;
        // Optional: if head is underwater, knees are implicitly underwater
        if (underwater) isKneeUnderwater = true;
    }

    public void SetKneeUnderwater(bool underwater)
    {
        isKneeUnderwater = underwater;
    }
}