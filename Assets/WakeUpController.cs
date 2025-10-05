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
    public Image fadeOverlay; // Drag the black UI panel here
    public float additionalFadeTime = 1f;

    [Header("References")]
    public Transform mainCamera; // Drag your MainCamera here
    public MonoBehaviour playerMovementScript; // Drag your movement script here

    [Header("Audio (Optional)")]
    public AudioSource audioSource;
    public AudioClip wakeUpSound;
    public AudioClip breathingSound;

    private bool isWakingUp = false;
    private bool hasWokenUp = false;

    void Start()
    {
        // Try to find MainCamera if not assigned
        if (mainCamera == null)
        {
            Transform fpCamera = transform.Find("FirstPersonCamera");
            if (fpCamera != null)
            {
                mainCamera = fpCamera.Find("MainCamera");
            }
        }

        // Start with black screen if fade overlay exists
        if (fadeOverlay != null)
        {
            Color color = fadeOverlay.color;
            color.a = 1f;
            fadeOverlay.color = color;
        }

        if (playOnStart)
        {
            // Small delay to ensure everything is initialized
            Invoke("StartWakeUpAnimation", 0.1f);
        }
    }

    public void StartWakeUpAnimation()
    {
        if (!isWakingUp && !hasWokenUp)
        {
            StartCoroutine(WakeUpSequence());
        }
    }

    IEnumerator WakeUpSequence()
    {
        isWakingUp = true;

        Debug.Log("Starting wake up animation...");

        // Disable player movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
            Debug.Log("Player movement disabled");
        }

        // Lock cursor during animation
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set initial rotations - lying down (90 degrees)
        Quaternion startRotation = Quaternion.Euler(90f, 0f, 0f);
        Quaternion endRotation = Quaternion.Euler(0f, 0f, 0f);

        if (mainCamera != null)
        {
            // Preserve the Y rotation (left/right look)
            float currentYRotation = mainCamera.localEulerAngles.y;
            startRotation = Quaternion.Euler(90f, currentYRotation, 0f);
            endRotation = Quaternion.Euler(0f, currentYRotation, 0f);

            mainCamera.localRotation = startRotation;
        }

        float totalTime = wakeUpDuration + additionalFadeTime;
        float elapsedTime = 0f;

        // Brief pause before starting
        yield return new WaitForSeconds(0.5f);

        // Play wake up sound
        if (audioSource != null && wakeUpSound != null)
        {
            audioSource.PlayOneShot(wakeUpSound);
        }

        while (elapsedTime < totalTime)
        {
            float progress = elapsedTime / totalTime;

            // Camera rotation (happens in first part of animation)
            if (elapsedTime < wakeUpDuration && mainCamera != null)
            {
                float rotationProgress = elapsedTime / wakeUpDuration;
                float curvedProgress = wakeUpCurve.Evaluate(rotationProgress);
                mainCamera.localRotation = Quaternion.Slerp(startRotation, endRotation, curvedProgress);
            }

            // Fade effect (happens throughout entire duration)
            if (fadeOverlay != null)
            {
                Color color = fadeOverlay.color;
                color.a = Mathf.Lerp(1f, 0f, progress);
                fadeOverlay.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finalize everything
        if (mainCamera != null)
        {
            mainCamera.localRotation = endRotation;
        }

        if (fadeOverlay != null)
        {
            Color color = fadeOverlay.color;
            color.a = 0f;
            fadeOverlay.color = color;
        }

        // Re-enable movement
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
            Debug.Log("Player movement re-enabled");
        }

        // Play breathing or ambient sound
        if (audioSource != null && breathingSound != null)
        {
            audioSource.clip = breathingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        hasWokenUp = true;
        isWakingUp = false;

        Debug.Log("Wake up animation complete!");
    }

    // Public method to check if animation is complete
    public bool HasWokenUp()
    {
        return hasWokenUp;
    }

    // Method to manually trigger wake up (useful for cutscenes)
    public void TriggerWakeUp()
    {
        playOnStart = false;
        hasWokenUp = false;
        StartWakeUpAnimation();
    }
}