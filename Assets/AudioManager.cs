using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource sfxSource; // For one-shot sound effects
    public AudioSource ambientSource; // For looping ambient sounds
    public AudioSource musicSource; // For background music

    [Header("Game Over Sounds")]
    public AudioClip gameOverSound; // Dramatic death sound
    public AudioClip healthDepletedSound; // Specific to health death
    public AudioClip oxygenDepletedSound; // Specific to oxygen death

    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip correctAnswerSound;
    public AudioClip wrongAnswerSound;
    public AudioClip objectiveUpdateSound;

    [Header("Pickup Sounds")]
    public AudioClip pickupTowelSound;
    public AudioClip pickupCatSound;
    public AudioClip pickupHeavyObjectSound;

    [Header("Fire Sounds")]
    public AudioClip catchFireSound; // When player catches fire
    public AudioClip fireExtinguishedSound; // After stop drop roll
    public AudioClip fireCrackleAmbient; // Looping background fire sound

    [Header("Stop Drop Roll Sounds")]
    public AudioClip dropSound; // Camera drops
    public AudioClip rollSound; // Each roll action

    [Header("Window Escape Sounds")]
    public AudioClip windowBreakSound;
    public AudioClip escapeSuccessSound;

    [Header("Environmental Sounds")]
    public AudioClip coughingSound;
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambientVolume = 0.5f;
    [Range(0f, 1f)] public float musicVolume = 0.3f;

    private static AudioManager instance;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Set initial volumes
        if (sfxSource != null) sfxSource.volume = sfxVolume;
        if (ambientSource != null) ambientSource.volume = ambientVolume;
        if (musicSource != null) musicSource.volume = musicVolume;

        // Start ambient fire sound if available
        PlayAmbientFire();
    }

    // === GAME OVER SOUNDS ===
    public static void PlayGameOver(bool isHealth)
    {
        if (instance == null) return;

        if (isHealth && instance.healthDepletedSound != null)
            instance.PlaySFX(instance.healthDepletedSound);
        else if (!isHealth && instance.oxygenDepletedSound != null)
            instance.PlaySFX(instance.oxygenDepletedSound);
        else if (instance.gameOverSound != null)
            instance.PlaySFX(instance.gameOverSound);
    }

    // === UI SOUNDS ===
    public static void PlayButtonClick()
    {
        if (instance != null && instance.buttonClickSound != null)
            instance.PlaySFX(instance.buttonClickSound);
    }

    public static void PlayCorrectAnswer()
    {
        if (instance != null && instance.correctAnswerSound != null)
            instance.PlaySFX(instance.correctAnswerSound);
    }

    public static void PlayWrongAnswer()
    {
        if (instance != null && instance.wrongAnswerSound != null)
            instance.PlaySFX(instance.wrongAnswerSound);
    }

    public static void PlayObjectiveUpdate()
    {
        if (instance != null && instance.objectiveUpdateSound != null)
            instance.PlaySFX(instance.objectiveUpdateSound);
    }

    // === PICKUP SOUNDS ===
    public static void PlayPickupTowel()
    {
        if (instance != null && instance.pickupTowelSound != null)
            instance.PlaySFX(instance.pickupTowelSound);
    }

    public static void PlayPickupCat()
    {
        if (instance != null && instance.pickupCatSound != null)
            instance.PlaySFX(instance.pickupCatSound);
    }

    public static void PlayPickupHeavyObject()
    {
        if (instance != null && instance.pickupHeavyObjectSound != null)
            instance.PlaySFX(instance.pickupHeavyObjectSound);
    }

    // === FIRE SOUNDS ===
    public static void PlayCatchFire()
    {
        if (instance != null && instance.catchFireSound != null)
            instance.PlaySFX(instance.catchFireSound);
    }

    public static void PlayFireExtinguished()
    {
        if (instance != null && instance.fireExtinguishedSound != null)
            instance.PlaySFX(instance.fireExtinguishedSound);
    }

    public void PlayAmbientFire()
    {
        if (ambientSource != null && fireCrackleAmbient != null)
        {
            ambientSource.clip = fireCrackleAmbient;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }

    public static void StopAmbientFire()
    {
        if (instance != null && instance.ambientSource != null)
            instance.ambientSource.Stop();
    }

    // === STOP DROP ROLL SOUNDS ===
    public static void PlayDrop()
    {
        if (instance != null && instance.dropSound != null)
            instance.PlaySFX(instance.dropSound);
    }

    public static void PlayRoll()
    {
        if (instance != null && instance.rollSound != null)
            instance.PlaySFX(instance.rollSound);
    }

    // === WINDOW ESCAPE SOUNDS ===
    public static void PlayWindowBreak()
    {
        if (instance != null && instance.windowBreakSound != null)
            instance.PlaySFX(instance.windowBreakSound);
    }

    public static void PlayEscapeSuccess()
    {
        if (instance != null && instance.escapeSuccessSound != null)
            instance.PlaySFX(instance.escapeSuccessSound);
    }

    // === CORE PLAYBACK METHODS ===
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // === VOLUME CONTROL ===
    public static void SetSFXVolume(float volume)
    {
        if (instance != null && instance.sfxSource != null)
        {
            instance.sfxVolume = volume;
            instance.sfxSource.volume = volume;
        }
    }

    public static void SetAmbientVolume(float volume)
    {
        if (instance != null && instance.ambientSource != null)
        {
            instance.ambientVolume = volume;
            instance.ambientSource.volume = volume;
        }
    }

    public static void SetMusicVolume(float volume)
    {
        if (instance != null && instance.musicSource != null)
        {
            instance.musicVolume = volume;
            instance.musicSource.volume = volume;
        }
    }
}