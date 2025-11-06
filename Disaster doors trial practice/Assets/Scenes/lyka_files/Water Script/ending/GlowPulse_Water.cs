using UnityEngine;

public class GlowPulse_Water : MonoBehaviour
{
    public Material mat;
    public Color glowColor = Color.cyan;
    public float pulseSpeed = 2f;
    public float minIntensity = 1f;
    public float maxIntensity = 5f;

    private void Update()
    {
        float emission = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        mat.SetColor("_EmissionColor", glowColor * emission);
    }
}