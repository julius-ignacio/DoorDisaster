using MilkShake;
using UnityEngine;
using System.Collections;

public class ConsistentQuake : MonoBehaviour
{
    public Shaker shaker;
    public ShakePreset shakePreset;

    private AudioSource audi;

    public float quakeInterval = 15f;  // cooldown between quakes
    public float quakeDuration = 8f;   // how long quake lasts

    private ShakeInstance currentShake;

    void Start()
    {
        audi = GetComponent<AudioSource>();
        StartCoroutine(QuakeRoutine());
    }

    IEnumerator QuakeRoutine()
    {
        while (true)
        {
            // 🔥 Start quake
            if (audi != null)
                audi.Play();

            if (shaker != null && shakePreset != null)
            {
                currentShake = shaker.Shake(shakePreset); // Start sustained shake
                Debug.Log("Earthquake started!");
            }

            // ⏳ Wait quake duration
            yield return new WaitForSeconds(quakeDuration);

            // 🛑 Stop quake
            if (currentShake != null)
            {
                currentShake.Stop(0.5f, true); // fade out over 0.5s
                currentShake = null;
                Debug.Log("Earthquake ended!");
            }

            if (audi != null)
                audi.Stop();

            // ⏳ Wait cooldown before next quake
            yield return new WaitForSeconds(quakeInterval);
        }
    }
}
