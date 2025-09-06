using MilkShake;
using UnityEngine;
using System.Collections;

public class ConsistentQuake : MonoBehaviour
{
    public GameObject quakeIcon;
    public Shaker shaker;
    private ShakeInstance currentShake;

    public Camera coverCam;
    public ShakePreset shakePreset;
    public PanicMeterScript panicMeterScript;

    public GameObject[] LockerNoises;

    private AudioSource audi;

    public float quakeInterval = 15f;  // cooldown between quakes
    public float quakeDuration = 10f;   // how long quake lasts

    public bool IsQuakeActive { get; private set; } = false;


    void Start()
    {
        quakeIcon.SetActive(false);
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
                quakeIcon.SetActive(true);

                Debug.Log("Earthquake started!");



                foreach (GameObject obj in LockerNoises)
                {
                    if (obj != null)
                    {
                        AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                        if (lockerAudio != null)
                        {
                            lockerAudio.Play();
                        }
                    }
                }
            }

            IsQuakeActive = true;

            // Panic increase during quake
            float elapsed = 0f;
            while (elapsed < quakeDuration)
            {
                if (!coverCam.enabled && panicMeterScript != null)
                    panicMeterScript.currHealth += Time.deltaTime * 2f; // adjust rate

                elapsed += Time.deltaTime;
                yield return null; // wait 1 frame
            }

            // 🛑 Stop quake
            if (currentShake != null)
            {
                currentShake.Stop(0, true);
                currentShake = null;
                quakeIcon.SetActive(false);

                Debug.Log("Earthquake ended!");

                    foreach (GameObject obj in LockerNoises)
                {
                    if (obj != null)
                    {
                        AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                        if (lockerAudio != null)
                        {
                            lockerAudio.Stop();
                        }
                    }
                }
            }
            audi?.Stop();

            IsQuakeActive = false;

            // ⏳ Wait cooldown before next quake
            yield return new WaitForSeconds(quakeInterval);
        }
    }

}
