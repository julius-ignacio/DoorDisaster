using UnityEngine;
using System.Linq;

public class LightManager_Water : MonoBehaviour
{
    private Light[] houseLights;
    private Renderer[] bulbRenderers;
    private bool lightsOn = true;

    [Header("Bulb Colors")]
    [SerializeField] private Color onColor = Color.white;
    [SerializeField] private Color offColor = Color.black;
    [SerializeField] private Color offAlbedo = new Color(0.2f, 0.2f, 0.2f);

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
}
