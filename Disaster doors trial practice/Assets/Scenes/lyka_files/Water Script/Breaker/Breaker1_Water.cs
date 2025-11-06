using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class Breaker1_Water : MonoBehaviour, IInteractable_Water
{
    [Header("Core References")]
    [SerializeField] private LightManager_Water lightManager; // Controls lights in the scene
    [SerializeField] private WaterRising waterRising;         // Controls the rising water mechanic
    [SerializeField] private Transform player;                // Reference to the player character

    [Header("Flood Warning UI")]
    [SerializeField] private TextMeshProUGUI floodWarningText; // Text that displays flood warnings or messages
    [SerializeField] private float floodWarningDuration = 5f;  // How long the flood warning stays on screen
    [SerializeField] private float textFadeDuration = 1f;      // How long it takes for text to fade in/out

    [Header("Event System")]
    public UnityEvent OnBreakerTurnedOff; // 🔊 Event for other scripts (e.g., RadioIntro_Water)

    private Coroutine textRoutine;              // Stores the currently running coroutine for text display
    private bool playerKilledByBreaker = false; // Prevents multiple deaths from the same hazard

    // The breaker starts with the power ON
    public bool IsPowerOn { get; private set; } = true;

    private void Awake()
    {
        // Check if this object has the "FuseBox" tag (important for interaction)
        if (!CompareTag("FuseBox"))
        {
            Debug.LogWarning($"⚠️ {name} should have tag 'FuseBox'!");
        }

        // Ensure collider is NOT a trigger, so the player can interact properly with raycasts
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger)
        {
            col.isTrigger = false;
            Debug.LogWarning($"{name}: Changed collider to non-Trigger.");
        }

        // Hide flood warning text initially
        if (floodWarningText != null)
            floodWarningText.alpha = 0f;

        // Automatically find the player if not manually assigned in Inspector
        player ??= GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        // Sync breaker state with the light system on scene start
        if (lightManager != null)
            IsPowerOn = lightManager.AreLightsOn();
        else
            IsPowerOn = true; // Default to ON if no manager found

        Debug.Log($"🔌 Breaker initial state: {(IsPowerOn ? "Power ON" : "Power OFF")}");
    }

    private void Update()
    {
        // Continuously check if the breaker (which has electricity) is under water
        if (IsPowerOn && waterRising != null && lightManager != null)
        {
            // If breaker is below or touching the water height...
            if (transform.position.y <= waterRising.waterMesh.position.y)
            {
                // ...and player hasn’t already been electrocuted, kill the player
                if (!playerKilledByBreaker)
                    KillPlayerByBreaker();
            }
        }
    }

    // Text prompt shown to the player when looking at the breaker
    public string GetPrompt()
    {
        if (lightManager == null)
            return "Press E: (No LightManager assigned)";

        return lightManager.AreLightsOn()
            ? "Press E: Switch OFF Power"
            : "Power is OFF";
    }

    // When the player interacts with the breaker
    public void Interact()
    {
        if (lightManager == null)
        {
            Debug.LogError($"{name}: No LightManager assigned!");
            return;
        }

        // The breaker can only be turned OFF, not turned back ON
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
        // Turn off the lights in the scene
        lightManager.TurnOffLights();
        IsPowerOn = false;
        Debug.Log("⚡ Breaker switched OFF.");

        // 🔊 Trigger event for other scripts (like RadioIntro_Water)
        OnBreakerTurnedOff?.Invoke();

        // Trigger the water rising mechanic once power is off
        if (waterRising != null)
        {
            waterRising.riseInterval = 25f; // Set interval between each water rise
            waterRising.StartFloodSequence(); // Begin flooding sequence
            Debug.Log("🌊 Flood sequence started (every 25 seconds)!");
        }

        // Display a warning on the screen
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine); // Stop any currently running message
            textRoutine = StartCoroutine(ShowFloodWarning("The Flood is Rising!"));
        }
    }

    // Coroutine that fades in and out a warning text
    private IEnumerator ShowFloodWarning(string message)
    {
        floodWarningText.text = message;

        // Fade text in
        float t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            floodWarningText.alpha = Mathf.Lerp(0f, 1f, t / textFadeDuration);
            yield return null;
        }

        floodWarningText.alpha = 1f;
        yield return new WaitForSeconds(floodWarningDuration); // Wait before fading out

        // Fade text out
        t = 0f;
        while (t < textFadeDuration)
        {
            t += Time.deltaTime;
            floodWarningText.alpha = Mathf.Lerp(1f, 0f, t / textFadeDuration);
            yield return null;
        }

        floodWarningText.alpha = 0f;
    }

    // Handles what happens when player gets electrocuted
    private void KillPlayerByBreaker()
    {
        if (playerKilledByBreaker) return; // Prevent multiple triggers
        playerKilledByBreaker = true;

        Debug.Log("⚡ Player electrocuted by breaker underwater!");

        // Show warning text about electrocution
        if (floodWarningText != null)
        {
            if (textRoutine != null)
                StopCoroutine(textRoutine);
            textRoutine = StartCoroutine(ShowFloodWarning("ELECTROCUTED! Don't turn on power underwater!"));
        }

        // Disable the player object (simulating death)
        if (player != null)
            player.gameObject.SetActive(false);

        // Restart the level after 3 seconds
        StartCoroutine(RestartSceneAfterDelay(3f));
    }

    // Wait a few seconds before reloading the scene
    private IEnumerator RestartSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Draws a small colored sphere above the breaker in Scene View to show its state
    private void OnDrawGizmos()
    {
        Gizmos.color = IsPowerOn ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 1f, 0.25f);
    }

    // Helper method to easily find the breaker by tag in the scene
    public static Breaker1_Water FindFuseBox()
    {
        GameObject fuseBoxObject = GameObject.FindGameObjectWithTag("FuseBox");
        if (fuseBoxObject != null)
            return fuseBoxObject.GetComponent<Breaker1_Water>();
        
        Debug.LogWarning("⚠️ No GameObject with tag 'FuseBox' found!");
        return null;
    }
}
