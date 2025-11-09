using MilkShake;
using UnityEngine;
using System.Collections;

public class ConsistentQuake : MonoBehaviour
{
    [Header("References")]
    public GameObject quakeIcon;
    public Shaker shaker;
    public ShakePreset shakePreset;
    public Camera coverCam;
    public PanicMeterScript panicMeterScript;
    public GameObject[] LockerNoises;

    [Header("Settings")]
    public float quakeInterval = 15f;  // cooldown between quakes
    public float quakeDuration = 10f;  // how long quake lasts

    private AudioSource audi;
    private ShakeInstance currentShake;
    private Coroutine quakeRoutine;

    private bool isPaused = false; // ✅ NEW — used to pause/resume
    public bool IsQuakeActive { get; private set; } = false;

    void Start()
    {
        quakeIcon.SetActive(false);
        audi = GetComponent<AudioSource>();
        quakeRoutine = StartCoroutine(QuakeRoutine());
    }

    // ✅ Call this to PAUSE quake (used during quiz)
    public void PauseQuakes()
    {
        isPaused = true;

        // stop shake visuals/audio temporarily
        if (currentShake != null)
            currentShake.Pause(0);

        foreach (GameObject obj in LockerNoises)
        {
            if (obj != null)
            {
                AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                if (lockerAudio != null && lockerAudio.isPlaying)
                    lockerAudio.Pause();
            }
        }

        audi?.Pause();
        Debug.Log("🌙 Quakes paused.");
    }

    // ✅ Call this to RESUME quake after quiz
    public void ResumeQuakes()
    {
        isPaused = false;

        if (currentShake != null)
            currentShake.Resume(0);

        foreach (GameObject obj in LockerNoises)
        {
            if (obj != null)
            {
                AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                if (lockerAudio != null)
                    lockerAudio.UnPause();
            }
        }

        audi?.UnPause();
        Debug.Log("☀️ Quakes resumed.");
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
                Debug.Log("🌋 Earthquake started!");

                foreach (GameObject obj in LockerNoises)
                {
                    if (obj != null)
                    {
                        AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                        if (lockerAudio != null)
                            lockerAudio.Play();
                    }
                }
            }

            IsQuakeActive = true;

            // ⏱ Shake duration
            float elapsed = 0f;
            while (elapsed < quakeDuration)
            {
                // ⏸ Wait while paused
                if (isPaused)
                {
                    yield return null;
                    continue;
                }

                if (!coverCam.enabled && panicMeterScript != null)
                    panicMeterScript.currHealth += Time.deltaTime * 2f;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 🛑 Stop quake visuals
            StopActiveQuake();

            // ⏳ Wait before next quake
            float wait = 0f;
            while (wait < quakeInterval)
            {
                if (isPaused)
                {
                    yield return null;
                    continue;
                }

                wait += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void StopActiveQuake()
    {
        if (currentShake != null)
        {
            currentShake.Stop(0, true);
            currentShake = null;
        }

        foreach (GameObject obj in LockerNoises)
        {
            if (obj != null)
            {
                AudioSource lockerAudio = obj.GetComponent<AudioSource>();
                if (lockerAudio != null) lockerAudio.Stop();
            }
        }

        audi?.Stop();
        quakeIcon.SetActive(false);
        IsQuakeActive = false;

        Debug.Log("🛑 Earthquake ended!");
    }
}
