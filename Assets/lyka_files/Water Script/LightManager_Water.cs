using UnityEngine;
using System.Linq;
using System.Collections;

public class LightManager_Water : MonoBehaviour
{
    private Light[] houseLights;
    private Renderer[] bulbRenderers;
    private bool lightsOn = true;

    [Header("Bulb Colors")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.black;
    [SerializeField] private Color offAlbedo = new Color(0.2f, 0.2f, 0.2f);

    [Header("Flicker Settings")]
    [SerializeField] private bool enableFlicker = false;
    [SerializeField] private float minFlickerInterval = 0.05f;
    [SerializeField] private float maxFlickerInterval = 0.3f;
    [SerializeField] private float flickerChance = 0.3f;
    [SerializeField] private float intensityVariation = 0.5f;

    [Header("Flicker Cycle Settings")]
    [SerializeField] private float minFlickerDuration = 2f;
    [SerializeField] private float maxFlickerDuration = 5f;
    [SerializeField] private float minBreakDuration = 2f;
    [SerializeField] private float maxBreakDuration = 6f;

    private void Awake()
    {
        houseLights = GameObject.FindGameObjectsWithTag("HouseLight")
                                .Select(go => go.GetComponent<Light>())
                                .Where(l => l != null)
                                .ToArray();

        bulbRenderers = GameObject.FindGameObjectsWithTag("Bulb")
                                  .Select(go => go.GetComponent<Renderer>())
                                  .Where(r => r != null)
                                  .ToArray();

        Debug.Log($"LightManager found {houseLights.Length} lights and {bulbRenderers.Length} bulbs.");

        ApplyLightState(lightsOn);

        if (enableFlicker)
            StartCoroutine(FlickerCycleRoutine());
    }

    public void TurnOffLights()
    {
        ApplyLightState(false);
        Debug.Log("Lights OFF");
    }

    public void TurnOnLights()
    {
        ApplyLightState(true);
        Debug.Log("Lights ON");
    }

    private void ApplyLightState(bool state)
    {
        foreach (Light light in houseLights)
            if (light != null) light.enabled = state;

        lightsOn = state;
        UpdateBulbVisuals(state);
    }

    private void UpdateBulbVisuals(bool isOn)
    {
        foreach (Renderer rend in bulbRenderers)
        {
            if (rend == null) continue;
            Material mat = rend.material;

            if (isOn)
            {
                mat.SetColor("_BaseColor", onColor);
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", onColor * 2f);
            }
            else
            {
                mat.SetColor("_BaseColor", offAlbedo);
                mat.SetColor("_EmissionColor", offColor);
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public bool AreLightsOn() => lightsOn;

    // ======================================================
    // ✨ Continuous Flicker System (with breaks)
    // ======================================================
    private IEnumerator FlickerCycleRoutine()
    {
        while (true)
        {
            if (lightsOn)
            {
                // --- Flicker active for a random duration ---
                float flickerTime = Random.Range(minFlickerDuration, maxFlickerDuration);
                float timer = 0f;

                while (timer < flickerTime && lightsOn)
                {
                    foreach (Light light in houseLights)
                    {
                        if (light != null && Random.value < flickerChance)
                        {
                            StartCoroutine(SmoothFlickerLight(light));
                        }
                    }

                    timer += Random.Range(minFlickerInterval, maxFlickerInterval);
                    yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));
                }

                // --- Break (steady light) ---
                float breakTime = Random.Range(minBreakDuration, maxBreakDuration);
                yield return new WaitForSeconds(breakTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    // 🌟 SMOOTH INTENSITY FLICKER (no hard blinking)
    private IEnumerator SmoothFlickerLight(Light light)
    {
        float originalIntensity = light.intensity;
        float targetIntensity = Mathf.Clamp(originalIntensity * Random.Range(1f - intensityVariation, 1f + intensityVariation), 0.1f, 8f);
        float duration = Random.Range(0.05f, 0.2f);
        float elapsed = 0f;

        // fade to target
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(originalIntensity, targetIntensity, elapsed / duration);
            yield return null;
        }

        // fade back to original
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            light.intensity = Mathf.Lerp(targetIntensity, originalIntensity, elapsed / duration);
            yield return null;
        }

        light.intensity = originalIntensity;
    }
}
