using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingTrigger_Water : MonoBehaviour
{
    // ===============================
    // 🔧 REFERENCES & SETTINGS
    // ===============================

    [Header("References")]
 // Reference to the objective system to mark mission complete
    public Transform teleportDestination;              // Position where the player will be teleported at the end
    public GameObject player;                          // The player GameObject
    public PlayerController_Water playerController;    // Controls player movement (used to disable control)
    public MouseMovement_Water mouseMovement;          // Controls camera look movement

    [Header("UI Elements")]
    // Various UI elements that need to be hidden during ending cinematic
    public GameObject oxygenUI;
    public GameObject heartUI;
    public GameObject joystickUI;
    public GameObject jumpButton;
    public GameObject inventoryUI;
    public GameObject timerUI;
    public GameObject dataUI;

    // Black fade overlay and text for cinematic messages
    public Image blackOverlay;
    public TextMeshProUGUI missionCompleteText;
    public TextMeshProUGUI narrationText;

    [Header("Cinematic Settings")]
    // Timings for different fade and delay effects during cinematic sequence
    public float fastFadeDuration = 0.8f;
    public float blackScreenDuration = 6f;
    public float fadeBackDuration = 1.5f;
    public float missionTextDelay = 1.5f;
    public float endDelay = 4f;
    public float narrationDisplayTime = 4f;

    [Header("Flood References")]
    // References to gameplay scripts controlling water, oxygen, and health systems
    public WaterRising waterRising;
    public PlayerOxygen_Water playerOxygen;
    public HeartSysWater heartSystem; // Player health (hearts) system

    [Header("Audio References")]
    // Audio clips used in cinematic (radio, helicopter, and rain)
    public AudioSource radioAudio;
    public AudioSource helicopterAudio;
    public AudioSource rainAudio;

    // Used to ensure the ending cinematic only triggers once
    private bool hasTriggered = false;

    // =================================
    // 🧠 INITIALIZATION
    // =================================
    void Start()
    {
        // Automatically find missing references in the scene
        
        if (!waterRising) waterRising = FindObjectOfType<WaterRising>();
        if (!playerOxygen) playerOxygen = FindObjectOfType<PlayerOxygen_Water>();
        if (!playerController) playerController = FindObjectOfType<PlayerController_Water>();
        if (!mouseMovement) mouseMovement = FindObjectOfType<MouseMovement_Water>();
        if (!heartSystem) heartSystem = FindObjectOfType<HeartSysWater>();

        // Setup black overlay (fade effect)
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(false); // start disabled
            blackOverlay.raycastTarget = false;       // don't block clicks initially
            var color = blackOverlay.color;
            color.a = 0f;                             // fully transparent
            blackOverlay.color = color;
        }

        // Hide mission/narration texts at start
        SetTextAlpha(missionCompleteText, 0f);
        SetTextAlpha(narrationText, 0f);
    }

    // =================================
    // 🚪 TRIGGER DETECTION
    // =================================
    private void OnTriggerEnter(Collider other)
    {
        // Only trigger once and only for the player
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;

        // Start the ending cinematic sequence
        StartCoroutine(PlayEndingSequence());
    }

    // =================================
    // 🎬 ENDING CINEMATIC SEQUENCE
    // =================================
    private IEnumerator PlayEndingSequence()
    {
        // Disable player control and camera movement
        if (playerController) playerController.enabled = false;
        if (mouseMovement) mouseMovement.enabled = false;

        // Activate the black overlay for fade effects
        if (blackOverlay) blackOverlay.gameObject.SetActive(true);

        // Stop the water rising system
        if (waterRising) waterRising.StopFloodSequence();

        // Stop oxygen depletion and ensure player is considered above water
        if (playerOxygen)
        {
            playerOxygen.ForceSurface(); // Custom method to force "surface state"
            playerOxygen.enabled = false; // Disable further oxygen updates
        }

        // Stop health loss and refill hearts completely
        if (heartSystem)
        {
            heartSystem.ResetHearts();   // Restore all hearts
            heartSystem.enabled = false; // Stop damage tracking
        }

        // Stop all audio sources except for cinematic ones
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            if (source != helicopterAudio && source != radioAudio && source != rainAudio)
                source.Stop();
        }

        // Hide all gameplay UIs for cinematic look
        HideUI(oxygenUI, heartUI, joystickUI, jumpButton, inventoryUI, timerUI, dataUI);

        // Fade to full black (simulate fade-out)
        yield return StartCoroutine(FadeOverlay(0f, 1f, fastFadeDuration, blockClicks: true));

        // Start cinematic ambient sounds
        if (helicopterAudio && !helicopterAudio.isPlaying) helicopterAudio.Play();
        if (radioAudio && !radioAudio.isPlaying) radioAudio.Play();
        if (rainAudio && !rainAudio.isPlaying) rainAudio.Play();

        // Display narration text (radio dialogue)
        if (narrationText)
        {
            narrationText.text = "This is Rescue Team Delta, we’ve got visual on the survivor.";
            yield return StartCoroutine(FadeTextIn(narrationText, 0.6f)); // Fade in text
            yield return new WaitForSeconds(narrationDisplayTime);        // Wait visible
            yield return StartCoroutine(FadeTextOut(narrationText, 0.6f)); // Fade out
        }

        // Keep the screen black for a bit longer before teleport
        yield return new WaitForSeconds(blackScreenDuration - narrationDisplayTime);

        // Teleport player to safe location (rescue spot)
        if (player && teleportDestination)
            player.transform.position = teleportDestination.position;

        // Fade back from black to the scene (simulate recovery)
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeBackDuration, blockClicks: false));

        // Re-enable movement, but keep most UI hidden for cinematic look
        if (playerController) playerController.enabled = true;
        if (mouseMovement) mouseMovement.enabled = true;

        // Only show movement controls again
        if (joystickUI) joystickUI.SetActive(true);
        if (jumpButton) jumpButton.SetActive(true);

        // Keep other UIs hidden for clean cinematic
        if (oxygenUI) oxygenUI.SetActive(false);
        if (heartUI) heartUI.SetActive(false);
        if (inventoryUI) inventoryUI.SetActive(false);
        if (dataUI) dataUI.SetActive(false);
        if (timerUI) timerUI.SetActive(false);

        // Hide objective text (since mission is ending)
       

        // Show "Mission Complete" text after delay
        yield return new WaitForSeconds(missionTextDelay);
        if (missionCompleteText)
            yield return StartCoroutine(FadeTextIn(missionCompleteText, 1f));

      

        // Wait then fade to black for end scene transition
        yield return new WaitForSeconds(endDelay);
        yield return StartCoroutine(FadeOverlay(0f, 1f, fastFadeDuration, blockClicks: true));
    }

    // =================================
    // 🧩 HELPER METHODS
    // =================================

    // Hides all given UI GameObjects
    private void HideUI(params GameObject[] uiObjects)
    {
        foreach (var ui in uiObjects)
        {
            if (ui) ui.SetActive(false);
        }
    }

    // Handles fade transitions for the black overlay (screen fade in/out)
    private IEnumerator FadeOverlay(float startAlpha, float endAlpha, float duration, bool blockClicks)
    {
        if (!blackOverlay) yield break;
        blackOverlay.raycastTarget = blockClicks;
        float elapsed = 0f;
        var color = blackOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            blackOverlay.color = color;
            yield return null;
        }

        // Ensure final alpha is exact
        color.a = endAlpha;
        blackOverlay.color = color;

        // If faded out fully, disable overlay
        if (endAlpha <= 0.01f)
        {
            blackOverlay.raycastTarget = false;
            blackOverlay.gameObject.SetActive(false);
        }
    }

    // Smoothly fades text in (for narration or mission text)
    private IEnumerator FadeTextIn(TextMeshProUGUI text, float duration)
    {
        if (!text) yield break;
        float elapsed = 0f;
        var color = text.color;
        color.a = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            text.color = color;
            yield return null;
        }

        color.a = 1f;
        text.color = color;
    }

    // Smoothly fades text out (for narration)
    private IEnumerator FadeTextOut(TextMeshProUGUI text, float duration)
    {
        if (!text) yield break;
        float elapsed = 0f;
        var color = text.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            text.color = color;
            yield return null;
        }

        color.a = 0f;
        text.color = color;
    }

    // Instantly set the transparency of a text element
    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        if (!text) return;
        var c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
