using UnityEngine;
using System.Collections;
using Narrate;
using UnityEngine.UI;

public class RadioIntro_Water : MonoBehaviour
{
    [Header("References")]
    public Transform radioTarget;             // The radio’s position in the scene that the player/camera should look at during the intro
    public AudioSource radioAudio1;           // First radio dialogue line
    public AudioSource radioAudio2;           // Second radio dialogue line
    public AudioSource radioAudio3;           // Third radio line triggered by breaker event
    public AudioSource gibberishAudio;        // Background static/gibberish sound before and after radio lines
    public PlayerController_Water playerController;   // Reference to the player movement controller script
    public MouseMovement_Water mouseMovement;         // Reference to camera/mouse look control
   // public NarrationTrigger_Water narrationTrigger;   // Handles subtitle/narration display timing for each radio line

    [Header("Objective Manager")]
    // (Left empty — could be used to update mission objectives later)

    [Header("Breaker Connection")]
    public Breaker1_Water breaker;            // Reference to the breaker switch to listen for On/Off events

    [Header("UI Control")]
    public GameObject uiRoot;                 // Root object of player’s HUD/UI (hidden during cinematic sequences)

    [Header("Timing Settings")]
    public float delayBeforeLook = 1f;        // Delay before the player automatically looks at the radio
    public float lookDuration = 2f;           // Time it takes to rotate the player/camera toward the radio
    public float returnDuration = 1.5f;       // Time it takes to rotate back to the original rotation

    [Header("Gibberish Timing")]
    public float gibberishStartDelay = 1f;    // Delay before static/gibberish resumes after dialogue ends
    public float gibberishResumeDelay = 0.5f; // Delay after breaker line before gibberish resumes

    [Header("Look Offset")]
    [Range(-2f, 2f)] public float horizontalOffset = 0f;  // Adjusts left/right direction offset when looking at radio
    [Range(-2f, 2f)] public float verticalOffset = -0.5f; // Adjusts up/down direction offset when looking at radio

    [Header("🌧️ Rain Audio Only")]
    public AudioSource rainAudio;             // Background looping rain sound to create ambiance
    public bool keepRainAcrossScenes = true;  // If true, rain audio will persist between scene loads

    private bool hasPlayedIntro = false;      // Ensures the radio intro sequence plays only once
    private bool hasPlayedBreakerLine = false;// Ensures the breaker-related line plays only once

    private const float NarrationDuration = 8f;      // Duration of subtitle/narration display for radio lines
    private const float Radio1SubtitleDelay = 4f;    // Delay before showing subtitles for the first line

    void Awake()
    {
        // If enabled, keep rain audio alive even if a new scene loads (persistent object)
        if (keepRainAcrossScenes && rainAudio != null)
        {
            DontDestroyOnLoad(rainAudio.gameObject);
        }
    }

    void OnEnable()
    {
        // Subscribe to breaker events (detect when it turns on or off)
        if (breaker != null)
        {
            breaker.OnBreakerTurnedOff?.AddListener(HandleBreakerOff); // When breaker is turned off, play radio line  // When breaker is turned on, stop all radio sounds
        }
    }

    void OnDisable()
    {
        // Unsubscribe from breaker events to prevent memory leaks
        if (breaker != null)
        {
            breaker.OnBreakerTurnedOff?.RemoveListener(HandleBreakerOff);
            
        }
    }

    void Start()
    {
        // 🌧️ Start looping background rain sound
        StartRainAudio();

        // 📻 Start the radio intro cinematic sequence when scene begins
        StartCoroutine(RadioIntroSequence());
    }

    // 🌧️----------------- RAIN AUDIO CONTROL -----------------🌧️
    private void StartRainAudio()
    {
        // Plays rain sound in a loop with volume control
        if (rainAudio != null && !rainAudio.isPlaying)
        {
            rainAudio.loop = true;
            rainAudio.volume = 0.4f;
            rainAudio.Play();
            Debug.Log("🎵 Continuous rain audio started looping.");
        }
    }

    private void StopRainAudio()
    {
        // Stops rain sound if playing
        if (rainAudio != null && rainAudio.isPlaying)
            rainAudio.Stop();
    }
    // 🌧️------------------------------------------------------🌧️

    private void HandleBreakerOff()
    {
        // Triggered when breaker is turned off
        if (!hasPlayedBreakerLine)
        {
            StartCoroutine(PlayBreakerRadioLine());  // Plays special radio message
            hasPlayedBreakerLine = true;             // Prevent multiple plays
        }
    }

    private void HandleBreakerOn()
    {
        // Stop all active sounds when breaker is reactivated
        StopAllAudio();
    }

    IEnumerator RadioIntroSequence()
    {
        // 🕹️ Disable player movement and UI during the intro
        if (playerController != null) playerController.canMove = false;
        if (mouseMovement != null) mouseMovement.enabled = false;
        if (uiRoot != null) uiRoot.SetActive(false);

        // Wait before looking at radio
        yield return new WaitForSeconds(delayBeforeLook);

        Quaternion originalRotation = transform.rotation; // Save player’s starting rotation

        // 🎯 Rotate toward the radio target for cinematic effect
        if (radioTarget != null)
        {
            // Calculate direction from player to radio
            Vector3 targetDir = (radioTarget.position - transform.position).normalized;
            targetDir.y += verticalOffset;
            targetDir.x += horizontalOffset;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

            // Smoothly rotate toward the radio
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / lookDuration;
                transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, t);
                yield return null;
            }
        }

        // --- 🎧 RADIO LINE 1 ---
        if (!hasPlayedIntro && radioAudio1 != null)
        {
            StartCoroutine(FadeOutGibberish(0.5f));  // Gradually fade out static before speech
            radioAudio1.Play();                      // Play first radio message

            // Trigger subtitle/narration for this line after a short delay
           // narrationTrigger?.PlayNarration("RadioLine1", Radio1SubtitleDelay, NarrationDuration);
            hasPlayedIntro = true;

            // Wait until this audio clip finishes
            yield return new WaitForSeconds(radioAudio1.clip.length);
        }

        // --- 🎧 RADIO LINE 2 ---
        if (radioAudio2 != null)
        {
            StartCoroutine(FadeOutGibberish(0.5f));  // Stop background noise again
            radioAudio2.Play();                      // Play second line
           // narrationTrigger?.PlayNarration("RadioLine2", 0f, NarrationDuration);
            yield return new WaitForSeconds(radioAudio2.clip.length);
        }

        // 🌀 Resume gibberish/static sound after radio lines
        if (gibberishAudio != null)
            StartCoroutine(PlayGibberishAfterDelay(gibberishStartDelay));

        // 🎥 Smoothly rotate back to the player’s original view
        float tReturn = 0f;
        Quaternion currentRotation = transform.rotation;
        while (tReturn < 1f)
        {
            tReturn += Time.deltaTime / returnDuration;
            transform.rotation = Quaternion.Slerp(currentRotation, originalRotation, tReturn);
            yield return null;
        }

        // ✅ Re-enable movement and UI after cinematic ends
        if (playerController != null) playerController.canMove = true;
        if (mouseMovement != null) mouseMovement.enabled = true;
        if (uiRoot != null) uiRoot.SetActive(true);
    }

    private IEnumerator PlayBreakerRadioLine()
    {
        // Plays when breaker is turned off
        StopAllAudio(); // Stop other audio before playing new one
        if (radioAudio3 != null)
        {
            StartCoroutine(FadeOutGibberish(0.5f));  // Mute static first
            radioAudio3.Play();                      // Play special breaker message
            //narrationTrigger?.PlayNarration("RadioLine3", 0f, NarrationDuration);
            Debug.Log("🎧 Breaker radio line playing...");
            yield return new WaitForSeconds(radioAudio3.clip.length);
        }

        // Resume gibberish after a short delay
        yield return new WaitForSeconds(gibberishResumeDelay);
        if (gibberishAudio != null)
        {
            gibberishAudio.loop = true;
            gibberishAudio.Play();
            Debug.Log("🎙️ Gibberish resumed after breaker radio line.");
        }
    }

    IEnumerator PlayGibberishAfterDelay(float delay)
    {
        // Wait before starting gibberish (after dialogue)
        yield return new WaitForSeconds(delay);
        if (gibberishAudio != null && !gibberishAudio.isPlaying)
        {
            gibberishAudio.loop = true;
            gibberishAudio.Play();
            Debug.Log("🎙️ Gibberish audio started looping after " + delay + " seconds.");
        }
    }

    IEnumerator FadeOutGibberish(float fadeDuration)
    {
        // Smoothly lowers gibberish volume before stopping
        if (gibberishAudio == null || !gibberishAudio.isPlaying) yield break;

        float startVolume = gibberishAudio.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            gibberishAudio.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        gibberishAudio.Stop();
        gibberishAudio.volume = startVolume; // Reset volume for future playback
    }

    private void StopAllAudio()
    {
        // Stops all radio and gibberish sounds to prevent overlaps
        if (radioAudio1 != null && radioAudio1.isPlaying) radioAudio1.Stop();
        if (radioAudio2 != null && radioAudio2.isPlaying) radioAudio2.Stop();
        if (radioAudio3 != null && radioAudio3.isPlaying) radioAudio3.Stop();
        if (gibberishAudio != null && gibberishAudio.isPlaying) gibberishAudio.Stop();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draws a visible line in the Scene view from this object to the radio target
        // Helps developers visualize the look direction during setup
        if (radioTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, radioTarget.position);
            Gizmos.DrawWireSphere(radioTarget.position, 0.25f);
        }
    }
#endif
}
