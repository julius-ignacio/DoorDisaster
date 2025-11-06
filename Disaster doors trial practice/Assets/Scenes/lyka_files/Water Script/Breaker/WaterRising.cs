using UnityEngine;
using TMPro;
using System.Collections;

public class WaterRising : MonoBehaviour
{
    [Header("Water Settings")]
    public Transform waterMesh;          // Reference to the water mesh or object that visually represents the water surface
    public float riseSpeed = 0.5f;       // Speed of water rising (for continuous mode)
    public float maxHeight = 5f;         // Maximum height the water can reach
    public bool useStepRising = true;    // Toggle between step-based rising or smooth continuous rising

    [Header("Step Rising Settings")]
    public float riseInterval = 25f;     // Time interval between each step rise (e.g., water rises every 25 seconds)
    public float riseAmount = 0.5f;      // How much the water rises per step

    [Header("Underwater Effects")]
    public Transform underwaterEffect;   // Optional visual effect that follows the water (e.g., bubbles, fog)
    public Transform underwaterVolume;   // Optional volume for underwater post-processing
    public float followOffset = 0.2f;    // Slight offset below water level to position underwater effects correctly

    [Header("Countdown UI")]
    public TMP_Text countdownText;       // UI text displaying countdown until next water rise

    [Header("UI Overlay (Optional)")]
    public CanvasGroup darkOverlay;      // Optional dark screen overlay when flood starts
    public TMP_Text floodWarningText;    // Text to warn player that the flood is rising

    [Header("Penalty / Reward Display")]
    public TMP_Text penaltyText;         // Text that shows penalties or rewards (e.g., "-5s" or "+10s")

    private bool isRising = false;       // Determines whether flood has started
    private bool overlayVisible = false; // Tracks if the overlay has been shown
    private float startY;                // Stores starting Y position of water (used to limit rise height)
    private float stepTimer;             // Countdown timer for step-based water rise

    void Start()
    {
        // If no water mesh is assigned, assume the object this script is on
        if (waterMesh == null) waterMesh = transform;

        // Store the starting water height
        startY = waterMesh.position.y;

        // Initialize step timer to start countdown
        stepTimer = riseInterval;
        UpdateCountdownText();

        // Hide UI elements at the start
        if (darkOverlay != null) darkOverlay.alpha = 0f;
        if (floodWarningText != null) floodWarningText.alpha = 0f;
        if (penaltyText != null) penaltyText.alpha = 0f;
    }

    void Update()
    {
        // Stop if flood has not started yet
        if (!isRising) return;

        // Choose between continuous rising or step rising
        if (useStepRising)
            HandleStepRising();
        else
            HandleContinuousRising();

        // Update underwater effects to follow the water
        UpdateUnderwaterEffectPosition();

        // Continuously update countdown UI
        UpdateCountdownText();
    }

    // 🌊 Smoothly raises water level every frame (continuous mode)
    private void HandleContinuousRising()
    {
        if (waterMesh.position.y < startY + maxHeight)
            waterMesh.position += Vector3.up * riseSpeed * Time.deltaTime;
    }

    // 🌊 Handles step-based water rising (rises after set intervals)
    private void HandleStepRising()
    {
        // Count down timer every second
        stepTimer -= Time.deltaTime;

        // When timer reaches 0, make water rise
        if (stepTimer <= 0f)
        {
            RiseWaterStep();
            stepTimer = riseInterval; // Reset timer
        }
    }

    // ⬆️ Raises water by a fixed amount each step
    private void RiseWaterStep()
    {
        if (waterMesh.position.y < startY + maxHeight)
        {
            // Calculate new position (limited by max height)
            Vector3 newPos = waterMesh.position + Vector3.up * riseAmount;
            newPos.y = Mathf.Min(newPos.y, startY + maxHeight);
            waterMesh.position = newPos;

            Debug.Log($"🌊 Flood rose by {riseAmount}! Current Y = {waterMesh.position.y}");

            // Show a warning message each time the flood rises
            StartCoroutine(ShowFloodWarning());
        }
    }

    // 💧 Keeps underwater visuals aligned with current water height
    private void UpdateUnderwaterEffectPosition()
    {
        if (underwaterEffect != null)
            underwaterEffect.position = new Vector3(waterMesh.position.x, waterMesh.position.y - followOffset, waterMesh.position.z);

        if (underwaterVolume != null)
            underwaterVolume.position = new Vector3(waterMesh.position.x, waterMesh.position.y - followOffset, waterMesh.position.z);
    }

    // ⏱️ Updates countdown timer text for player UI
    private void UpdateCountdownText()
    {
        if (countdownText == null) return;

        if (!useStepRising)
        {
            countdownText.text = "";
            return;
        }

        countdownText.text = isRising
            ? $"Next Rise: {Mathf.Max(stepTimer, 0f):F1}s"  // Shows countdown in seconds
            : "Flood not started";                          // Message when flood hasn’t begun
    }

    // 🚨 Begins the flood rising sequence
    public void StartFloodSequence()
    {
        isRising = true;
        stepTimer = riseInterval; // Reset timer
    }

    // ⛔ Applies penalty from quiz (shortens countdown, making water rise faster)
    public void ApplyPenaltyFromQuiz(float seconds)
    {
        if (!isRising) StartFloodSequence();
        if (!useStepRising) return;

        // Subtract time to make flood rise sooner
        stepTimer -= seconds;
        if (stepTimer < 0f) stepTimer = 0f;
        UpdateCountdownText();

        // Display red penalty text animation
        if (penaltyText != null)
            StartCoroutine(ShowFloatingText($"-{seconds:F0}s", Color.red));

        // Shake countdown text for visual feedback
        if (countdownText != null)
            StartCoroutine(ShakeCountdown());
    }

    // ✅ Applies reward from quiz (extends countdown, delaying next water rise)
    public void ApplyRewardFromQuiz(float seconds)
    {
        if (!isRising) StartFloodSequence();
        if (!useStepRising) return;

        // Add time to delay the next flood step
        stepTimer += seconds;
        stepTimer = Mathf.Min(stepTimer, riseInterval * 2f); // Cap to avoid excessive delay
        UpdateCountdownText();

        // Display green reward text animation
        if (penaltyText != null)
            StartCoroutine(ShowFloatingText($"+{seconds:F0}s", Color.green));

        // Shake countdown for effect
        if (countdownText != null)
            StartCoroutine(ShakeCountdown());
    }

    // 💬 Displays floating text animation for penalties or rewards
    private IEnumerator ShowFloatingText(string text, Color color)
    {
        penaltyText.text = text;
        penaltyText.color = color;
        penaltyText.alpha = 1f;
        penaltyText.gameObject.SetActive(true);

        // Floating text animation parameters
        Vector3 startPos = penaltyText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0);
        float duration = 1f;
        float t = 0f;

        // Smoothly move text upward and fade out
        while (t < duration)
        {
            t += Time.deltaTime;
            penaltyText.transform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            penaltyText.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        // Reset after animation
        penaltyText.alpha = 0f;
        penaltyText.transform.localPosition = startPos;
    }

    // ⚠️ Displays warning message and dark overlay when flood rises
    private IEnumerator ShowFloodWarning()
    {
        // Fade in dark overlay only once
        if (darkOverlay != null && !overlayVisible)
        {
            overlayVisible = true;
            yield return StartCoroutine(FadeUI(darkOverlay, 1f, 0.5f));
        }

        // Show warning text temporarily
        if (floodWarningText != null)
        {
            floodWarningText.text = "The Flood is Rising!";
            floodWarningText.alpha = 1f;
            yield return new WaitForSeconds(2f);
            floodWarningText.alpha = 0f;
        }
    }

    // 💢 Small shake animation for countdown text
    private IEnumerator ShakeCountdown()
    {
        Vector3 originalPos = countdownText.transform.localPosition;
        float shakeDuration = 0.3f;
        float shakeStrength = 5f;
        float timer = 0f;

        // Random small movement around original position
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            countdownText.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeStrength;
            yield return null;
        }

        // Reset position after shaking
        countdownText.transform.localPosition = originalPos;
    }

    // 🌓 Smooth fade in/out animation for UI overlays
    private IEnumerator FadeUI(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float t = 0f;

        // Interpolate between current and target alpha
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        cg.alpha = target; // Ensure final value is exact
    }

    // 🛑 Stops flood rising and hides overlay
    public void StopFloodSequence()
    {
        isRising = false;

        // Fade out overlay when flood stops
        if (darkOverlay != null)
            StartCoroutine(FadeUI(darkOverlay, 0f, 1f));

        overlayVisible = false;
    }
}
