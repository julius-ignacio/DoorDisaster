using UnityEngine;
using System.Linq;
using System.Collections;

public class LightManager_Water : MonoBehaviour
{
    // Array to store all light components tagged as "HouseLight"
    private Light[] houseLights;

    // Array to store all bulb renderers tagged as "Bulb" (for visual effect)
    private Renderer[] bulbRenderers;

    // Tracks whether lights are currently ON or OFF
    private bool lightsOn = true;

    [Header("Bulb Colors")]
    [SerializeField] private Color onColor = Color.white;            // Color of bulb when ON
    [SerializeField] private Color offColor = Color.black;           // Emission color when OFF
    [SerializeField] private Color offAlbedo = new Color(0.2f, 0.2f, 0.2f); // Base color when OFF (dim gray look)

    [Header("Flicker Settings")]
    [SerializeField] private bool enableFlicker = false;             // Toggle for flickering effect
    [SerializeField] private float minFlickerInterval = 0.05f;       // Minimum time between flickers
    [SerializeField] private float maxFlickerInterval = 0.3f;        // Maximum time between flickers
    [SerializeField] private float flickerChance = 0.3f;             // Probability a light will flicker
    [SerializeField] private float intensityVariation = 0.5f;        // How much brightness changes during flicker

    [Header("Flicker Cycle Settings")]
    [SerializeField] private float minFlickerDuration = 2f;          // Minimum duration lights will keep flickering
    [SerializeField] private float maxFlickerDuration = 5f;          // Maximum duration of flickering period
    [SerializeField] private float minBreakDuration = 2f;            // Minimum duration lights stay stable
    [SerializeField] private float maxBreakDuration = 6f;            // Maximum duration of stable light period

    private void Awake()
    {
        // 🔦 Find all lights tagged as "HouseLight" in the scene
        // Uses LINQ to filter objects that actually have a Light component
        houseLights = GameObject.FindGameObjectsWithTag("HouseLight")
                                .Select(go => go.GetComponent<Light>())
                                .Where(l => l != null)
                                .ToArray();

        // 💡 Find all bulbs tagged as "Bulb" to control their material visuals
        bulbRenderers = GameObject.FindGameObjectsWithTag("Bulb")
                                  .Select(go => go.GetComponent<Renderer>())
                                  .Where(r => r != null)
                                  .ToArray();

        // Debug info: how many lights and bulbs were found
        Debug.Log($"LightManager found {houseLights.Length} lights and {bulbRenderers.Length} bulbs.");

        // Apply the initial light state (default ON)
        ApplyLightState(lightsOn);

        // If flicker feature is enabled, start the flicker routine
        if (enableFlicker)
            StartCoroutine(FlickerCycleRoutine());
    }

    // 🔌 Turn off all lights and update visuals
    public void TurnOffLights()
    {
        ApplyLightState(false);
        Debug.Log("Lights OFF");
    }

    // 💡 Turn on all lights and update visuals
    public void TurnOnLights()
    {
        ApplyLightState(true);
        Debug.Log("Lights ON");
    }

    // 🧠 Applies the given ON/OFF state to all lights and bulbs
    private void ApplyLightState(bool state)
    {
        // Enable or disable all light components
        foreach (Light light in houseLights)
            if (light != null) light.enabled = state;

        // Update stored state
        lightsOn = state;

        // Update bulb materials to match current light state
        UpdateBulbVisuals(state);
    }

    // 🎨 Visually updates the bulb materials based on ON/OFF state
    private void UpdateBulbVisuals(bool isOn)
    {
        foreach (Renderer rend in bulbRenderers)
        {
            if (rend == null) continue; // Skip null renderers
            Material mat = rend.material; // Access material instance for this bulb

            if (isOn)
            {
                // Set bright color and enable emission glow
                mat.SetColor("_BaseColor", onColor);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", onColor * 2f); // Multiply color to make glow brighter
            }
            else
            {
                // Set dim color and disable emission for "off" look
                mat.SetColor("_BaseColor", offAlbedo);
                mat.SetColor("_EmissionColor", offColor);
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    // 📢 Public method used by other scripts to check if lights are ON
    public bool AreLightsOn() => lightsOn;

    // ======================================================
    // ✨ Continuous Flicker System (with breaks)
    // ======================================================
    private IEnumerator FlickerCycleRoutine()
    {
        // Runs indefinitely as long as the script is active
        while (true)
        {
            if (lightsOn)
            {
                // --- Active flicker phase ---
                float flickerTime = Random.Range(minFlickerDuration, maxFlickerDuration); // Random duration for flickering
                float timer = 0f;

                // During flicker phase
                while (timer < flickerTime && lightsOn)
                {
                    foreach (Light light in houseLights)
                    {
                        // Each light has a chance to flicker each cycle
                        if (light != null && Random.value < flickerChance)
                        {
                            // Start smooth flicker effect for this specific light
                            StartCoroutine(SmoothFlickerLight(light));
                        }
                    }

                    // Wait for a random interval before next flicker attempt
                    timer += Random.Range(minFlickerInterval, maxFlickerInterval);
                    yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));
                }

                // --- Break phase (no flickering, lights stay steady) ---
                float breakTime = Random.Range(minBreakDuration, maxBreakDuration);
                yield return new WaitForSeconds(breakTime);
            }
            else
            {
                // If lights are off, wait for next frame and recheck
                yield return null;
            }
        }
    }

    // 🌟 Smoothly changes the brightness of a single light to simulate realistic flickering
    private IEnumerator SmoothFlickerLight(Light light)
    {
        float originalIntensity = light.intensity; // Store original light brightness
        // Random target intensity (slightly brighter or dimmer)
        float targetIntensity = Mathf.Clamp(originalIntensity * Random.Range(1f - intensityVariation, 1f + intensityVariation), 0.1f, 8f);
        float duration = Random.Range(0.05f, 0.2f); // Duration of flicker transition
        float elapsed = 0f;

        // --- Fade light intensity to target value ---
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(originalIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        // --- Fade light intensity back to original value ---
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(targetIntensity, originalIntensity, elapsed / duration);
            yield return null;
        }

        // Ensure final value resets perfectly to original brightness
        light.intensity = originalIntensity;
    }
}
