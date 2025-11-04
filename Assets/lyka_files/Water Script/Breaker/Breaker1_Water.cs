using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Breaker1_Water : MonoBehaviour, IInteractable_Water
{
    [Header("References")]
    [SerializeField] private LightManager_Water lightManager;
    [SerializeField] private WaterRising waterRising;
    [SerializeField] private ObjectiveManager_Water objectiveManager;
    [SerializeField] private Transform player;
    [SerializeField] private PlayerOxygen_Water playerOxygen;
    [SerializeField] private QuizTriggerManager_Water quizTriggerManager;

    [Header("Post Processing (Optional)")]
    [SerializeField] private Volume postProcessVolume;

    [Header("Flood Warning UI")]
    [SerializeField] private TextMeshProUGUI floodWarningText;
    [SerializeField] private float floodWarningDuration = 5f;
    [SerializeField] private float textFadeDuration = 1f;

    [Header("Setup")]
    [SerializeField] private bool ensureNonTriggerCollider = true;

    [Header("Breaker Events")]
    public UnityEvent OnBreakerTurnedOn;
    public UnityEvent OnBreakerTurnedOff;

    private ColorAdjustments colorAdjustments;
    private Coroutine textRoutine;
    private bool playerKilledByBreaker = false;

    // ✅ Public getter so other scripts can safely access
    public LightManager_Water LightManager => lightManager;

    // ✅ Start with power ON by default
    public bool IsPowerOn { get; private set; } = true;

    private void Reset()
    {
        lightManager ??= FindObjectOfType<LightManager_Water>();
        waterRising ??= FindObjectOfType<WaterRising>();
        postProcessVolume ??= FindObjectOfType<Volume>();
        objectiveManager ??= FindObjectOfType<ObjectiveManager_Water>();
        playerOxygen ??= FindObjectOfType<PlayerOxygen_Water>();
        quizTriggerManager ??= FindObjectOfType<QuizTriggerManager_Water>();
    }

    private void Awake()
    {
        // ✅ Verify the GameObject has "FuseBox" tag
        if (!CompareTag("FuseBox"))
        {
            Debug.LogWarning($"⚠️ {name} should have tag 'FuseBox' for proper identification!");
        }

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

        if (floodWarningText != null)
            floodWarningText.alpha = 0f;

        objectiveManager ??= FindObjectOfType<ObjectiveManager_Water>();
        player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        // 🧩 Sync breaker with light manager on start
        if (lightManager != null)
            IsPowerOn = lightManager.AreLightsOn();
        else
            IsPowerOn = true; // Default ON if no LightManager found

        Debug.Log($"🔌 Breaker initial state synced: {(IsPowerOn ? "Power ON" : "Power OFF")}");
    }

    private void Update()
    {
        // ⚡ Check if breaker is underwater while power is ON (electrocution hazard)
        if (lightManager != null && lightManager.AreLightsOn())
        {
            if (waterRising != null && transform.position.y <= waterRising.waterMesh.position.y)
            {
                if (!playerKilledByBreaker)
                    KillPlayerByBreaker();
            }
        }
    }

    // ✅ Modified to remove "Switch ON" prompt
    public string GetPrompt()
    {
        if (lightManager == null)
            return "Press E: (No LightManager assigned)";

        return lightManager.AreLightsOn()
            ? "Press E: Switch OFF Power"
            : "Power is OFF";
    }

    // ✅ Modified so it can only be turned OFF (not back ON)
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
            Debug.Log("⚡ Breaker is already OFF. Cannot turn it back on.");
        }
    }

    private void TurnOffBreaker()
    {
        lightManager.TurnOffLights();
        IsPowerOn = false;
        Debug.Log("⚡ Breaker switched OFF.");
        OnBreakerTurnedOff?.Invoke();

        // 🟢 Start water rising when breaker is OFF
        if (waterRising != null)
        {
            waterRising.riseInterval = 25f;
            waterRising.StartFloodSequence();
            Debug.Log("🌊 Flood sequence started (every 25 seconds)!");
        }

        // 🧩 Activate quizzes when breaker is OFF
        if (quizTriggerManager != null)
        {
            quizTriggerManager.ActivateQuizzes();
            Debug.Log("🧩 All quiz triggers activated after breaker OFF!");
        }

        // 🟠 Show flood warning
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("The Flood is Rising!"));
        }

        // ✅ Complete breaker-related objectives
        if (objectiveManager != null)
        {
            objectiveManager.CompleteMainObjective("Turn off the Breaker");
            objectiveManager.CompleteMainObjective("Turn off the power");
            objectiveManager.CompleteMainObjective("Go to the Basement");
            Debug.Log("✅ Breaker objectives completed");
        }
    }

    private void TurnOnBreaker()
    {
        lightManager.TurnOnLights();
        IsPowerOn = true;
        Debug.Log("⚡ Breaker switched ON.");
        OnBreakerTurnedOn?.Invoke();

        // 💡 Optional feedback
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("Breaker ON - Power Restored"));
        }

        // ⚡ Check if breaker is already underwater (instant electrocution)
        if (waterRising != null && transform.position.y <= waterRising.waterMesh.position.y)
            KillPlayerByBreaker();
    }

    private IEnumerator ShowFloodWarning(string message)
    {
        floodWarningText.text = message;

        // Fade in
        float t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            floodWarningText.alpha = Mathf.Lerp(0f, 1f, t / textFadeDuration);
            yield return null;
        }

        floodWarningText.alpha = 1f;
        yield return new WaitForSeconds(floodWarningDuration);

        // Fade out
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

        // Show death message
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("ELECTROCUTED! Don't turn on power underwater!"));
        }

        if (playerOxygen != null)
            playerOxygen.SendMessage("HandleDeath", SendMessageOptions.DontRequireReceiver);
        else if (player != null)
            player.gameObject.SetActive(false);

        StartCoroutine(RestartSceneAfterDelay(3f));
    }

    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 🧭 Gizmo: shows if breaker power is ON or OFF in Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = IsPowerOn ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, 0.25f);
    }

    // 🔍 Helper method to find this breaker by tag from other scripts
    public static Breaker1_Water FindFuseBox()
    {
        GameObject fuseBoxObject = GameObject.FindGameObjectWithTag("FuseBox");
        if (fuseBoxObject != null)
            return fuseBoxObject.GetComponent<Breaker1_Water>();
        
        Debug.LogWarning("⚠️ No GameObject with tag 'FuseBox' found!");
        return null;
    }

    [ContextMenu("Test Turn OFF")]
    private void TestTurnOff()
    {
        TurnOffBreaker();
    }
}