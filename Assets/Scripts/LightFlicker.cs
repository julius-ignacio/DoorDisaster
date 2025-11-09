using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light targetLight;


    public float minIntensity = .5f;
    public float maxIntensity = 5.0f;
    public float flickerSpeed = 0.1f;

    private void Start()
    {
        targetLight = GetComponent<Light>();

        InvokeRepeating("Flicker", 0f, flickerSpeed);
    }

    private void Flicker()
    {
        float randomIntensity = Random.Range(minIntensity, maxIntensity);
        targetLight.intensity = randomIntensity;
    }
}
