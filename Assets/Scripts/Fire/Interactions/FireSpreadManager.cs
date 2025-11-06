using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FireSpreadManager : MonoBehaviour
{
    [Header("Fire Spread Settings")]
    [Tooltip("Time in seconds between each wave of fires")]
    public float spreadInterval = 50f;

    [Header("Fire Waves")]
    [Tooltip("Organize fires into waves. Each wave activates together after the spread interval.")]
    public List<FireWave> fireWaves = new List<FireWave>();

    [Header("Audio")]
    public int fireSpreadSFX = 40;

    [Header("Debug")]
    public bool showDebugLogs = true;

    private int currentWaveIndex = 0;
    private bool isSpreadingActive = false;
    private bool isPaused = false;
    private Coroutine spreadCoroutine;

    public static FireSpreadManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DeactivateAllFires();
        AdjustDifficultySettings();
    }

    void AdjustDifficultySettings()
    {
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.currentStage == 0)
            {
                // Normal Mode - Slower fire spread
                spreadInterval = 50f; // More time between waves

                if (showDebugLogs)
                    Debug.Log("🔥 Fire Difficulty: NORMAL - Slower spread");
            }
            else
            {
                // Hard Mode - Faster fire spread
                spreadInterval = 25f; // Less time between waves

                if (showDebugLogs)
                    Debug.Log("🔥 Fire Difficulty: HARD - Faster spread");
            }
        }
    }

    void Update()
    {
        if (BreakerPuzzle.BreakerPuzzleComplete && !isSpreadingActive)
        {
            StartFireSpread();
        }
    }

    public void StartFireSpread()
    {
        if (isSpreadingActive) return;

        isSpreadingActive = true;

        if (showDebugLogs)
            Debug.Log($"Fire spreading started! First wave will ignite in {spreadInterval} seconds");

        spreadCoroutine = StartCoroutine(SpreadFireWaves());
    }

    IEnumerator SpreadFireWaves()
    {
        // Activate Wave 0 (First wave) after first interval
        yield return new WaitForSeconds(spreadInterval);

        if (currentWaveIndex < fireWaves.Count)
        {
            ActivateWave(currentWaveIndex);
            currentWaveIndex++;
        }

        if (showDebugLogs)
            Debug.Log("First wave activated. Pausing fire spread until player returns from House B...");

        // ⏸️ PAUSE HERE - Wait until ResumeFireSpread() is called from MrKittyPickup
        isPaused = true;

        while (isPaused)
        {
            yield return null;
        }

        if (showDebugLogs)
            Debug.Log("Fire spreading resumed! Continuing with remaining waves...");

        // Continue with remaining waves
        while (currentWaveIndex < fireWaves.Count)
        {
            yield return new WaitForSeconds(spreadInterval);

            ActivateWave(currentWaveIndex);
            currentWaveIndex++;
        }

        if (showDebugLogs)
            Debug.Log("All fire waves have spread!");
    }

    void ActivateWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= fireWaves.Count)
            return;

        FireWave wave = fireWaves[waveIndex];

        if (showDebugLogs)
            Debug.Log($"🔥 Wave {waveIndex + 1} ({wave.waveName}) activating with {wave.fires.Count} fire(s)!");

        // Activate all fires in this wave
        foreach (FireLocation fire in wave.fires)
        {
            if (fire != null)
            {
                ActivateFire(fire);
            }
        }
    }

    void ActivateFire(FireLocation fire)
    {
        // Activate particle system
        if (fire.fireParticle != null)
        {
            fire.fireParticle.gameObject.SetActive(true);
            fire.fireParticle.Play();
        }

        // Activate damage zone
        if (fire.fireZone != null)
        {
            fire.fireZone.SetActive(true);
        }

        // Activate light
        if (fire.fireLight != null)
        {
            fire.fireLight.enabled = true;
        }

        // Play sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(fireSpreadSFX);
        }

        if (showDebugLogs)
            Debug.Log($"  ├─ {fire.locationName} ignited!");
    }

    void DeactivateAllFires()
    {
        foreach (FireWave wave in fireWaves)
        {
            foreach (FireLocation fire in wave.fires)
            {
                if (fire.fireParticle != null)
                {
                    fire.fireParticle.gameObject.SetActive(false);
                    fire.fireParticle.Stop();
                }

                if (fire.fireZone != null)
                {
                    fire.fireZone.SetActive(false);
                }

                if (fire.fireLight != null)
                {
                    fire.fireLight.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Call this from MrKittyPickup after player teleports back to House A
    /// </summary>
    public void ResumeFireSpread()
    {
        if (isPaused)
        {
            isPaused = false;

            if (showDebugLogs)
                Debug.Log("FireSpreadManager: Resuming fire spread!");
        }
    }

    public void StopFireSpread()
    {
        if (spreadCoroutine != null)
        {
            StopCoroutine(spreadCoroutine);
            spreadCoroutine = null;
        }

        isSpreadingActive = false;
        isPaused = false;

        if (showDebugLogs)
            Debug.Log("Fire spreading stopped!");
    }

    public void ResetFireSpread()
    {
        StopFireSpread();
        DeactivateAllFires();
        currentWaveIndex = 0;
        isSpreadingActive = false;
        isPaused = false;

        if (showDebugLogs)
            Debug.Log("Fire spread system reset!");
    }

    // Manual trigger for specific wave (optional)
    public void TriggerWave(int waveIndex)
    {
        ActivateWave(waveIndex);
    }
}

[System.Serializable]
public class FireWave
{
    [Tooltip("Name for this wave (e.g., 'First Wave', 'Kitchen Area')")]
    public string waveName = "Fire Wave";

    [Tooltip("All fires that activate together in this wave")]
    public List<FireLocation> fires = new List<FireLocation>();
}

[System.Serializable]
public class FireLocation
{
    [Tooltip("Name of this fire location (e.g., 'Kitchen', 'Living Room')")]
    public string locationName = "Fire";

    [Header("Fire Components")]
    public ParticleSystem fireParticle;
    public GameObject fireZone;
    public Light fireLight;
}