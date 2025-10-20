using UnityEngine;
using TMPro; // 👈 For TextMeshPro UI

public class WaterRising : MonoBehaviour
{
    [Header("Water Settings")]
    public Transform waterMesh;
    public float riseSpeed = 0.5f;
    public float maxHeight = 5f;
    public bool useStepRising = false;

    [Header("Step Rising Settings")]
    public float riseInterval = 30f;
    public float riseAmount = 0.5f;

    [Header("Underwater Effects")]
    public Transform underwaterEffect;
    public Transform underwaterVolume;
    public float followOffset = 0.2f;

    [Header("Countdown UI")]
    public TMP_Text countdownText;       // ⏱️ Assign a TextMeshPro UI element here

    private bool isRising = false;
    private float startY;
    private float stepTimer;

    void Start()
    {
        if (waterMesh == null) waterMesh = transform;
        startY = waterMesh.position.y;
        stepTimer = riseInterval;

        UpdateCountdownText(); // show initial value
    }

    void Update()
    {
        if (!isRising) return;

        if (useStepRising)
            HandleStepRising();
        else
            HandleContinuousRising();

        UpdateUnderwaterEffectPosition();
        UpdateCountdownText(); // ⏱️ Update text every frame
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
            if (waterMesh.position.y < startY + maxHeight)
            {
                waterMesh.position = new Vector3(
                    waterMesh.position.x,
                    waterMesh.position.y + riseAmount,
                    waterMesh.position.z
                );
            }

            stepTimer = riseInterval; // Reset timer after rising
        }
    }

    private void UpdateUnderwaterEffectPosition()
    {
        if (underwaterEffect != null)
        {
            underwaterEffect.position = new Vector3(
                waterMesh.position.x,
                waterMesh.position.y - followOffset,
                waterMesh.position.z
            );
        }

        if (underwaterVolume != null)
        {
            underwaterVolume.position = new Vector3(
                waterMesh.position.x,
                waterMesh.position.y - followOffset,
                waterMesh.position.z
            );
        }
    }

    private void UpdateCountdownText()
    {
        if (countdownText == null) return;

        if (!useStepRising || !isRising)
        {
            countdownText.text = "";
            return;
        }

        // Show countdown in seconds (e.g. “Next Rise: 12.3s”)
        countdownText.text = $"Next Rise: {stepTimer:F1}s";
    }

    public void StartFloodSequence()
    {
        isRising = true;
        stepTimer = riseInterval;
    }
}
