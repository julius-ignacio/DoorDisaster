using UnityEngine;
using System.Collections;
using Narrate;
using UnityEngine.UI;

public class RadioIntro_Water : MonoBehaviour
{
    [Header("References")]
    public Transform radioTarget;
    public AudioSource radioAudio1;
    public AudioSource radioAudio2;
    public AudioSource radioAudio3;
    public AudioSource gibberishAudio;
    public PlayerController_Water playerController;
    public MouseMovement mouseMovement;
    public NarrationTrigger_Water narrationTrigger;

    [Header("Objective Manager")]
    public ObjectiveManager_Water objectiveManager;

    [Header("Breaker Connection")]
    public Breaker1_Water breaker;

    [Header("UI Control")]
    public GameObject uiRoot;

    [Header("Timing Settings")]
    public float delayBeforeLook = 1f;
    public float lookDuration = 2f;
    public float returnDuration = 1.5f;

    [Header("Gibberish Timing")]
    public float gibberishStartDelay = 1f;
    public float gibberishResumeDelay = 0.5f;

    [Header("Look Offset")]
    [Range(-2f, 2f)] public float horizontalOffset = 0f;
    [Range(-2f, 2f)] public float verticalOffset = -0.5f;

    [Header("🌧️ Rain Audio Only")]
    public AudioSource rainAudio;          // assign a looping rain sound
    public bool keepRainAcrossScenes = true;

    private bool hasPlayedIntro = false;
    private bool hasPlayedBreakerLine = false;

    private const float NarrationDuration = 8f;
    private const float Radio1SubtitleDelay = 4f;

    void Awake()
    {
        // ✅ Optional: make rain persist between scenes
        if (keepRainAcrossScenes && rainAudio != null)
        {
            DontDestroyOnLoad(rainAudio.gameObject);
        }
    }

    void OnEnable()
    {
        if (breaker != null)
        {
            breaker.OnBreakerTurnedOff?.AddListener(HandleBreakerOff);
            breaker.OnBreakerTurnedOn?.AddListener(HandleBreakerOn);
        }
    }

    void OnDisable()
    {
        if (breaker != null)
        {
            breaker.OnBreakerTurnedOff?.RemoveListener(HandleBreakerOff);
            breaker.OnBreakerTurnedOn?.RemoveListener(HandleBreakerOn);
        }
    }

    void Start()
    {
        // 🌧️ Start continuous rain sound
        StartRainAudio();

        // 📻 Begin radio intro sequence
        StartCoroutine(RadioIntroSequence());
    }

    // 🌧️----------------- RAIN AUDIO CONTROL -----------------🌧️
    private void StartRainAudio()
    {
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
        if (rainAudio != null && rainAudio.isPlaying)
            rainAudio.Stop();
    }
    // 🌧️------------------------------------------------------🌧️

    private void HandleBreakerOff()
    {
        if (!hasPlayedBreakerLine)
        {
            StartCoroutine(PlayBreakerRadioLine());
            hasPlayedBreakerLine = true;
        }
    }

    private void HandleBreakerOn()
    {
        StopAllAudio();
    }

    IEnumerator RadioIntroSequence()
    {
        // 🕹️ Disable controls + hide UI
        if (playerController != null) playerController.canMove = false;
        if (mouseMovement != null) mouseMovement.enabled = false;
        if (uiRoot != null) uiRoot.SetActive(false);

        yield return new WaitForSeconds(delayBeforeLook);

        Quaternion originalRotation = transform.rotation;

        // 🎯 Look toward the radio
        if (radioTarget != null)
        {
            Vector3 targetDir = (radioTarget.position - transform.position).normalized;
            targetDir.y += verticalOffset;
            targetDir.x += horizontalOffset;

            Quaternion targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);

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
            StartCoroutine(FadeOutGibberish(0.5f));
            radioAudio1.Play();

            narrationTrigger?.PlayNarration("RadioLine1", Radio1SubtitleDelay, NarrationDuration);
            hasPlayedIntro = true;
            yield return new WaitForSeconds(radioAudio1.clip.length);
        }

        // --- 🎧 RADIO LINE 2 ---
        if (radioAudio2 != null)
        {
            StartCoroutine(FadeOutGibberish(0.5f));
            radioAudio2.Play();
            narrationTrigger?.PlayNarration("RadioLine2", 0f, NarrationDuration);
            yield return new WaitForSeconds(radioAudio2.clip.length);
        }

        // 🌀 Resume gibberish
        if (gibberishAudio != null)
            StartCoroutine(PlayGibberishAfterDelay(gibberishStartDelay));

        // 🎥 Return camera
        float tReturn = 0f;
        Quaternion currentRotation = transform.rotation;
        while (tReturn < 1f)
        {
            tReturn += Time.deltaTime / returnDuration;
            transform.rotation = Quaternion.Slerp(currentRotation, originalRotation, tReturn);
            yield return null;
        }

        // ✅ Restore player control & UI
        if (playerController != null) playerController.canMove = true;
        if (mouseMovement != null) mouseMovement.enabled = true;
        if (uiRoot != null) uiRoot.SetActive(true);

        // ✅ Show objectives immediately after intro ends
        if (objectiveManager != null)
        {
            objectiveManager.ShowObjectivesNow();
            Debug.Log("🎯 Objectives displayed right after radio intro ends.");
        }

        Debug.Log("📻 Radio intro finished — player control & UI restored.");
    }

    private IEnumerator PlayBreakerRadioLine()
    {
        StopAllAudio();
        if (radioAudio3 != null)
        {
            StartCoroutine(FadeOutGibberish(0.5f));
            radioAudio3.Play();
            narrationTrigger?.PlayNarration("RadioLine3", 0f, NarrationDuration);
            Debug.Log("🎧 Breaker radio line playing...");
            yield return new WaitForSeconds(radioAudio3.clip.length);
        }

        // Resume gibberish
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
        gibberishAudio.volume = startVolume;
    }

    private void StopAllAudio()
    {
        if (radioAudio1 != null && radioAudio1.isPlaying) radioAudio1.Stop();
        if (radioAudio2 != null && radioAudio2.isPlaying) radioAudio2.Stop();
        if (radioAudio3 != null && radioAudio3.isPlaying) radioAudio3.Stop();
        if (gibberishAudio != null && gibberishAudio.isPlaying) gibberishAudio.Stop();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (radioTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, radioTarget.position);
            Gizmos.DrawWireSphere(radioTarget.position, 0.25f);
        }
    }
#endif
}
