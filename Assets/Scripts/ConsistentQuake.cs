using MilkShake;
using UnityEngine;
using System.Collections;

public class ConsistentQuake : MonoBehaviour
{
    public Shaker shaker;
    public Camera coverCam;
    public ShakePreset shakePreset;
    public PanicMeterScript panicMeterScript;

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
        audi?.Play();
        if (shaker != null && shakePreset != null)
        {
            currentShake = Shaker.ShakeAll(shakePreset);
            Debug.Log("Earthquake started!");
        }

        // Panic increase during quake
        float elapsed = 0f;
        while (elapsed < quakeDuration)
        {
            if (!coverCam.enabled && panicMeterScript != null)
                panicMeterScript.currHealth += Time.deltaTime * 10f; // adjust rate

            elapsed += Time.deltaTime;
            yield return null; // wait 1 frame
        }

        // 🛑 Stop quake
        if (currentShake != null)
        {
            currentShake.Stop(0, true); 
            currentShake = null;
            Debug.Log("Earthquake ended!");
        }
        audi?.Stop();

        // ⏳ Wait cooldown before next quake
        yield return new WaitForSeconds(quakeInterval);
    }
}

}
