using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingTrigger_Water : MonoBehaviour
{
    [Header("References")]
    public ObjectiveManager_Water objectiveManager;
    public Transform teleportDestination;
    public GameObject player;
    public PlayerController_Water playerController;
    public MouseMovement mouseMovement;

    [Header("UI Elements")]
    public GameObject oxygenUI;
    public GameObject heartUI;
    public GameObject joystickUI;
    public GameObject jumpButton;
    public GameObject inventoryUI;
    public GameObject timerUI;
    public GameObject dataUI;

    public Image blackOverlay;
    public TextMeshProUGUI missionCompleteText;
    public TextMeshProUGUI narrationText;

    [Header("Cinematic Settings")]
    public float fastFadeDuration = 0.8f;
    public float blackScreenDuration = 6f;
    public float fadeBackDuration = 1.5f;
    public float missionTextDelay = 1.5f;
    public float endDelay = 4f;
    public float narrationDisplayTime = 4f;

    [Header("Flood References")]
    public WaterRising waterRising;
    public PlayerOxygen_Water playerOxygen;
    public HeartSysWater heartSystem; // ✅ Correct reference to your heart script

    [Header("Audio References")]
    public AudioSource radioAudio;
    public AudioSource helicopterAudio;
    public AudioSource rainAudio;

    private bool hasTriggered = false;

    void Start()
    {
        if (!objectiveManager) objectiveManager = FindObjectOfType<ObjectiveManager_Water>();
        if (!waterRising) waterRising = FindObjectOfType<WaterRising>();
        if (!playerOxygen) playerOxygen = FindObjectOfType<PlayerOxygen_Water>();
        if (!playerController) playerController = FindObjectOfType<PlayerController_Water>();
        if (!mouseMovement) mouseMovement = FindObjectOfType<MouseMovement>();
        if (!heartSystem) heartSystem = FindObjectOfType<HeartSysWater>();

        // Overlay setup
        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(false);
            blackOverlay.raycastTarget = false;
            var color = blackOverlay.color;
            color.a = 0f;
            blackOverlay.color = color;
        }

        SetTextAlpha(missionCompleteText, 0f);
        SetTextAlpha(narrationText, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player")) return;
        hasTriggered = true;
        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        // Disable movement + look
        if (playerController) playerController.enabled = false;
        if (mouseMovement) mouseMovement.enabled = false;

        if (blackOverlay) blackOverlay.gameObject.SetActive(true);

        // Stop flood and oxygen logic
        if (waterRising) waterRising.StopFloodSequence();

        // ✅ FIXED: Use public method instead of private variable
        if (playerOxygen)
        {
            playerOxygen.ForceSurface(); // ✅ replaces direct access to isUnderwater
            playerOxygen.enabled = false;
        }

        // ✅ Stop losing hearts and restore them fully
        if (heartSystem)
        {
            heartSystem.ResetHearts();   // refill all hearts
            heartSystem.enabled = false; // stop any damage updates
        }

        // Stop all non-cinematic audio
        foreach (var source in FindObjectsOfType<AudioSource>())
        {
            if (source != helicopterAudio && source != radioAudio && source != rainAudio)
                source.Stop();
        }

        // --- Hide specific UIs ---
        HideUI(oxygenUI, heartUI, joystickUI, jumpButton, inventoryUI, timerUI, dataUI);

        // --- Fade to black ---
        yield return StartCoroutine(FadeOverlay(0f, 1f, fastFadeDuration, blockClicks: true));

        // --- Play cinematic sounds ---
        if (helicopterAudio && !helicopterAudio.isPlaying) helicopterAudio.Play();
        if (radioAudio && !radioAudio.isPlaying) radioAudio.Play();
        if (rainAudio && !rainAudio.isPlaying) rainAudio.Play();

        // --- Show narration ---
        if (narrationText)
        {
            narrationText.text = "This is Rescue Team Delta, we’ve got visual on the survivor.";
            yield return StartCoroutine(FadeTextIn(narrationText, 0.6f));
            yield return new WaitForSeconds(narrationDisplayTime);
            yield return StartCoroutine(FadeTextOut(narrationText, 0.6f));
        }

        // --- Stay black while sounds continue ---
        yield return new WaitForSeconds(blackScreenDuration - narrationDisplayTime);

        // --- Teleport player ---
        if (player && teleportDestination)
            player.transform.position = teleportDestination.position;

        // --- Fade back to scene ---
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeBackDuration, blockClicks: false));

        // ✅ Re-enable controls but keep main UI hidden for cinematic feel
        if (playerController) playerController.enabled = true;
        if (mouseMovement) mouseMovement.enabled = true;

        // Only bring back movement controls if desired
        if (joystickUI) joystickUI.SetActive(true);
        if (jumpButton) jumpButton.SetActive(true);

        // ❌ Keep these hidden
        if (oxygenUI) oxygenUI.SetActive(false);
        if (heartUI) heartUI.SetActive(false);
        if (inventoryUI) inventoryUI.SetActive(false);
        if (dataUI) dataUI.SetActive(false);
        if (timerUI) timerUI.SetActive(false);

        // Also hide objective text if available
        if (objectiveManager && objectiveManager.objectiveText)
            objectiveManager.objectiveText.gameObject.SetActive(false);

        // --- Mission Complete ---
        yield return new WaitForSeconds(missionTextDelay);
        if (missionCompleteText)
            yield return StartCoroutine(FadeTextIn(missionCompleteText, 1f));

        if (objectiveManager)
            objectiveManager.CompleteObjective("Escape");

        // --- Wait and fade to final black ---
        yield return new WaitForSeconds(endDelay);
        yield return StartCoroutine(FadeOverlay(0f, 1f, fastFadeDuration, blockClicks: true));
    }

    // --- Helpers ---
    private void HideUI(params GameObject[] uiObjects)
    {
        foreach (var ui in uiObjects)
        {
            if (ui) ui.SetActive(false);
        }
    }

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

        color.a = endAlpha;
        blackOverlay.color = color;

        if (endAlpha <= 0.01f)
        {
            blackOverlay.raycastTarget = false;
            blackOverlay.gameObject.SetActive(false);
        }
    }

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

    private void SetTextAlpha(TextMeshProUGUI text, float alpha)
    {
        if (!text) return;
        var c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
