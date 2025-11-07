using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WakeUpController : MonoBehaviour
{
    [Header("Wake Up Animation")]
    public float wakeUpDuration = 4f;
    public AnimationCurve wakeUpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool playOnStart = true;

    [Header("Visual Effects")]
    public Image fadeOverlay; 
    public float additionalFadeTime = 1f;

    [Header("References")]
    public Transform mainCamera; 
    public Movements2 movements; 

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip wakeUpSound;
    public AudioClip breathingSound;

    private bool isWakingUp = false;
    private bool hasWokenUp = false;

    void Start()
    {
        if (mainCamera == null)
        {
            Transform fpCamera = transform.Find("FirstPersonCamera");
            if (fpCamera != null)
                mainCamera = fpCamera.Find("MainCamera");
        }

        if (fadeOverlay != null)
        {
            Color color = fadeOverlay.color;
            color.a = 1f;
            fadeOverlay.color = color;
        }

        if (playOnStart)
            Invoke(nameof(StartWakeUpAnimation), 0.1f);
    }

    public void StartWakeUpAnimation()
    {
        if (!isWakingUp && !hasWokenUp)
            StartCoroutine(WakeUpSequence());
    }

    IEnumerator WakeUpSequence()
    {
        isWakingUp = true;

        if (movements != null) movements.enabled = false;

        Quaternion startRotation = Quaternion.Euler(-60f, 0f, 0f);
        Quaternion endRotation   = Quaternion.Euler(0f, 0f, 0f);

        if (mainCamera != null)
        {
            float currentY = mainCamera.localEulerAngles.y;
            startRotation = Quaternion.Euler(-60f, currentY, 0f);
            endRotation   = Quaternion.Euler(0f,    currentY, 0f);
            mainCamera.localRotation = startRotation;
        }

        float totalTime = wakeUpDuration + additionalFadeTime;
        float elapsedTime = 0f;

        yield return new WaitForSeconds(0.5f);

        if (audioSource != null && wakeUpSound != null)
            audioSource.PlayOneShot(wakeUpSound);

        while (elapsedTime < totalTime)
        {
            float progress = elapsedTime / totalTime;

            if (elapsedTime < wakeUpDuration && mainCamera != null)
            {
                float rotationProgress = elapsedTime / wakeUpDuration;
                float curved = wakeUpCurve.Evaluate(rotationProgress);
                mainCamera.localRotation = Quaternion.Slerp(startRotation, endRotation, curved);
            }

            if (fadeOverlay != null)
            {
                Color color = fadeOverlay.color;
                color.a = Mathf.Lerp(1f, 0f, progress);
                fadeOverlay.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
            mainCamera.localRotation = endRotation;

        if (fadeOverlay != null)
        {
            Color color = fadeOverlay.color;
            color.a = 0f;
            fadeOverlay.color = color;
        }

        if (movements != null) movements.enabled = true;

        if (audioSource != null && breathingSound != null)
        {
            audioSource.clip = breathingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        hasWokenUp = true;
        isWakingUp = false;

        // === IMPORTANT: Mark intro complete and persist that this script is done ===
        SubtitleManager2.ForceIntroComplete();

        // Disable the wake-up script so SavableFlag captures enabled=false and it stays skipped on reload
        enabled = false;
    }

    public bool HasWokenUp() => hasWokenUp;

    public void TriggerWakeUp()
    {
        playOnStart = false;
        hasWokenUp = false;
        StartWakeUpAnimation();
    }
}