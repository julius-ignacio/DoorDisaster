using UnityEngine;
using TMPro;
using System.Collections;

public class WaterRising : MonoBehaviour
{
    [Header("Water Settings")]
    public Transform waterMesh;
    public float riseSpeed = 0.5f;
    public float maxHeight = 5f;
    public bool useStepRising = true;

    [Header("Step Rising Settings")]
    public float riseInterval = 25f; // ✅ Changed to 25 seconds
    public float riseAmount = 0.5f;

    [Header("Underwater Effects")]
    public Transform underwaterEffect;
    public Transform underwaterVolume;
    public float followOffset = 0.2f;

    [Header("Countdown UI")]
    public TMP_Text countdownText;

    [Header("UI Overlay (Optional)")]
    public CanvasGroup darkOverlay;
    public TMP_Text floodWarningText;

    [Header("Penalty / Reward Display")]
    public TMP_Text penaltyText;

    private bool isRising = false;
    private bool overlayVisible = false;
    private float startY;
    private float stepTimer;

    void Start()
    {
        if (waterMesh == null) waterMesh = transform;
        startY = waterMesh.position.y;
        stepTimer = riseInterval;
        UpdateCountdownText();

        if (darkOverlay != null) darkOverlay.alpha = 0f;
        if (floodWarningText != null) floodWarningText.alpha = 0f;
        if (penaltyText != null) penaltyText.alpha = 0f;
    }

    void Update()
    {
        if (!isRising) return;

        if (useStepRising)
            HandleStepRising();
        else
            HandleContinuousRising();

        UpdateUnderwaterEffectPosition();
        UpdateCountdownText();
    }

    private void HandleContinuousRising()
    {
        if (waterMesh.position.y < startY + maxHeight)
            waterMesh.position += Vector3.up * riseSpeed * Time.deltaTime;
    }

    private void HandleStepRising()
    {
        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            RiseWaterStep();
            stepTimer = riseInterval;
        }
    }

    private void RiseWaterStep()
    {
        if (waterMesh.position.y < startY + maxHeight)
        {
            Vector3 newPos = waterMesh.position + Vector3.up * riseAmount;
            newPos.y = Mathf.Min(newPos.y, startY + maxHeight);
            waterMesh.position = newPos;

            Debug.Log($"🌊 Flood rose by {riseAmount}! Current Y = {waterMesh.position.y}");
            StartCoroutine(ShowFloodWarning());
        }
    }

    private void UpdateUnderwaterEffectPosition()
    {
        if (underwaterEffect != null)
            underwaterEffect.position = new Vector3(waterMesh.position.x, waterMesh.position.y - followOffset, waterMesh.position.z);

        if (underwaterVolume != null)
            underwaterVolume.position = new Vector3(waterMesh.position.x, waterMesh.position.y - followOffset, waterMesh.position.z);
    }

    private void UpdateCountdownText()
    {
        if (countdownText == null) return;

        if (!useStepRising)
        {
            countdownText.text = "";
            return;
        }

        countdownText.text = isRising
            ? $"Next Rise: {Mathf.Max(stepTimer, 0f):F1}s"
            : "Flood not started";
    }

    public void StartFloodSequence()
    {
        isRising = true;
        stepTimer = riseInterval;
    }

    public void ApplyPenaltyFromQuiz(float seconds)
    {
        if (!isRising) StartFloodSequence();
        if (!useStepRising) return;

        stepTimer -= seconds;
        if (stepTimer < 0f) stepTimer = 0f;
        UpdateCountdownText();

        if (penaltyText != null)
            StartCoroutine(ShowFloatingText($"-{seconds:F0}s", Color.red));
        if (countdownText != null)
            StartCoroutine(ShakeCountdown());
    }

    public void ApplyRewardFromQuiz(float seconds)
    {
        if (!isRising) StartFloodSequence();
        if (!useStepRising) return;

        stepTimer += seconds;
        stepTimer = Mathf.Min(stepTimer, riseInterval * 2f);
        UpdateCountdownText();

        if (penaltyText != null)
            StartCoroutine(ShowFloatingText($"+{seconds:F0}s", Color.green));
        if (countdownText != null)
            StartCoroutine(ShakeCountdown());
    }

    private IEnumerator ShowFloatingText(string text, Color color)
    {
        penaltyText.text = text;
        penaltyText.color = color;
        penaltyText.alpha = 1f;
        penaltyText.gameObject.SetActive(true);

        Vector3 startPos = penaltyText.transform.localPosition;
        Vector3 endPos = startPos + new Vector3(0, 50f, 0);
        float duration = 1f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            penaltyText.transform.localPosition = Vector3.Lerp(startPos, endPos, t / duration);
            penaltyText.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }

        penaltyText.alpha = 0f;
        penaltyText.transform.localPosition = startPos;
    }

    private IEnumerator ShowFloodWarning()
    {
        if (darkOverlay != null && !overlayVisible)
        {
            overlayVisible = true;
            yield return StartCoroutine(FadeUI(darkOverlay, 1f, 0.5f));
        }

        if (floodWarningText != null)
        {
            floodWarningText.text = "The Flood is Rising!";
            floodWarningText.alpha = 1f;
            yield return new WaitForSeconds(2f);
            floodWarningText.alpha = 0f;
        }
    }

    private IEnumerator ShakeCountdown()
    {
        Vector3 originalPos = countdownText.transform.localPosition;
        float shakeDuration = 0.3f;
        float shakeStrength = 5f;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            countdownText.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeStrength;
            yield return null;
        }

        countdownText.transform.localPosition = originalPos;
    }

    private IEnumerator FadeUI(CanvasGroup cg, float target, float duration)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        cg.alpha = target;
    }

    public void StopFloodSequence()
    {
        isRising = false;
        if (darkOverlay != null)
            StartCoroutine(FadeUI(darkOverlay, 0f, 1f));
        overlayVisible = false;
    }
}
