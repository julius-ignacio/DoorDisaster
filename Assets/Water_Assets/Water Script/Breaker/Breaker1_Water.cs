using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class Breaker1_Water : MonoBehaviour, IInteractable_Water
{
    [Header("References")]
    [SerializeField] private LightManager_Water lightManager;
    [SerializeField] private WaterRising waterRising;
    [SerializeField] private ObjectiveManager_Water objectiveManager;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerOxygen_Water playerOxygen;

    [Header("Post Processing (Optional)")]
    [SerializeField] private Volume postProcessVolume;

    [Header("UI Overlay (Optional)")]
    [SerializeField] private CanvasGroup darkOverlay;

    [Header("Flood Warning UI")]
    [SerializeField] private TextMeshProUGUI floodWarningText;
    [SerializeField] private float floodWarningDuration = 5f;
    [SerializeField] private float textFadeDuration = 1f;

    [Header("Setup")]
    [SerializeField] private bool ensureNonTriggerCollider = true;

    private ColorAdjustments colorAdjustments;
    private Coroutine fadeRoutine;
    private Coroutine textRoutine;
    private bool playerKilledByBreaker = false;

    private void Reset()
    {
        lightManager ??= FindObjectOfType<LightManager_Water>();
        waterRising ??= FindObjectOfType<WaterRising>();
        postProcessVolume ??= FindObjectOfType<Volume>();
        objectiveManager ??= FindObjectOfType<ObjectiveManager_Water>();
        playerOxygen ??= FindObjectOfType<PlayerOxygen_Water>();
    }

    private void Awake()
    {
        if (ensureNonTriggerCollider)
        {
            Collider col = GetComponent<Collider>();
            if (col != null && col.isTrigger)
            {
                col.isTrigger = false;
                Debug.LogWarning($"{name}: Collider was Trigger. Changed to non-Trigger for raycast interaction.");
            }
        }

        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out colorAdjustments))
            colorAdjustments.postExposure.value = 0f;

        if (darkOverlay != null)
        {
            darkOverlay.alpha = 0f;
            darkOverlay.blocksRaycasts = false;
            darkOverlay.interactable = false;
        }

        if (floodWarningText != null)
            floodWarningText.alpha = 0f;

        objectiveManager ??= FindObjectOfType<ObjectiveManager_Water>();
        player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (lightManager != null && lightManager.AreLightsOn())
        {
            if (waterRising != null && transform.position.y <= waterRising.waterMesh.position.y)
            {
                if (!playerKilledByBreaker)
                {
                    KillPlayerByBreaker();
                }
            }
        }
    }

    public string GetPrompt()
    {
        if (lightManager == null)
            return "Press E: (No LightManager assigned)";

        return lightManager.AreLightsOn()
            ? "Press E: Switch OFF Power"
            : "Press E: Switch ON Power";
    }

    public void Interact()
    {
        if (lightManager == null)
        {
            Debug.LogError($"{name}: No LightManager assigned!");
            return;
        }

        if (lightManager.AreLightsOn())
        {
            TurnOffBreaker();
        }
        else
        {
            TurnOnBreaker();
        }
    }

    private void TurnOffBreaker()
    {
        lightManager.TurnOffLights();
        Debug.Log("⚡ Breaker switched OFF.");

        // 🟢 Start water rising when breaker is OFF
        if (waterRising != null)
        {
            waterRising.StartFloodSequence();
            Debug.Log("🌊 Flood sequence started!");
        }

        // 🟠 Show the flood warning message here
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("The Flood is Rising!"));
        }

        SetDarkMode(true);

        if (objectiveManager != null)
        {
            objectiveManager.CompleteObjective("Turn off the Breaker");
        }
    }

    private void TurnOnBreaker()
    {
        lightManager.TurnOnLights();
        Debug.Log("⚡ Breaker switched ON.");

        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("Breaker ON - Power Restored"));
        }

        SetDarkMode(false);

        if (waterRising != null && transform.position.y <= waterRising.waterMesh.position.y)
        {
            KillPlayerByBreaker();
        }
    }

    private void SetDarkMode(bool enable)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        if (darkOverlay != null)
        {
            darkOverlay.blocksRaycasts = false;
            darkOverlay.interactable = false;
            fadeRoutine = StartCoroutine(FadeUI(darkOverlay, enable ? 1f : 0f, 1f));
        }
        else if (colorAdjustments != null)
        {
            fadeRoutine = StartCoroutine(FadeExposure(enable ? -3f : 0f, 1f));
        }
    }

    private IEnumerator FadeUI(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeExposure(float targetExposure, float duration)
    {
        if (colorAdjustments == null)
            yield break;

        float startExposure = colorAdjustments.postExposure.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            colorAdjustments.postExposure.value = Mathf.Lerp(startExposure, targetExposure, t);
            yield return null;
        }

        colorAdjustments.postExposure.value = targetExposure;
    }

    private IEnumerator ShowFloodWarning(string message)
    {
        floodWarningText.text = message;
        floodWarningText.alpha = 0f;

        float t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            floodWarningText.alpha = Mathf.Lerp(0f, 1f, t / textFadeDuration);
            yield return null;
        }

        floodWarningText.alpha = 1f;
        yield return new WaitForSeconds(floodWarningDuration);

        t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            floodWarningText.alpha = Mathf.Lerp(1f, 0f, t / textFadeDuration);
            yield return null;
        }

        floodWarningText.alpha = 0f;
    }

    private void KillPlayerByBreaker()
    {
        if (playerKilledByBreaker) return;
        playerKilledByBreaker = true;

        Debug.Log("⚡ Player electrocuted by breaker underwater!");

        if (playerOxygen != null)
        {
            playerOxygen.SendMessage("HandleDeath", SendMessageOptions.DontRequireReceiver);
        }
        else if (player != null)
        {
            player.gameObject.SetActive(false);
        }

        StartCoroutine(RestartSceneAfterDelay(2f));
    }

    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
