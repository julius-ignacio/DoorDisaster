using System.Collections;
using UnityEngine;

// Runs after WorldLoader, reconciles objectives with actual world state.
// It doesn't fight existing scripts; it nudges gates and objectives only when needed.
[DefaultExecutionOrder(1200)]
public class SequenceManager : MonoBehaviour
{
    [Header("Optional references (auto-find if empty)")]
    public SubtitleManager2 subtitleManager;
    public ObjectiveManager objectiveManager;
    public EmergencyHotlineCall hotline;
    public MrKittyPickup mrKitty;

    // Optional world models for quick state-derivation on load (leave null if not used)
    [Header("Optional world models (for fallback state checks)")]
    public GameObject phoneModel;
    public GameObject backpackModel;

    // Guards to avoid duplicate prompts
    private bool fixedIntro;
    private bool fixedAfterHotline;
    private bool fixedAfterDoorWithTowel;
    private bool fixedRescuePush;
    private bool fixedPackingStart;
    public GameManager gameManager;

    private void Awake()
    {
        if (subtitleManager == null)  subtitleManager = FindObjectOfType<SubtitleManager2>(true);
        if (objectiveManager == null) objectiveManager = FindObjectOfType<ObjectiveManager>(true);
        if (hotline == null)          hotline = FindObjectOfType<EmergencyHotlineCall>(true);
        if (mrKitty == null)          mrKitty = FindObjectOfType<MrKittyPickup>(true);
    }

    private void Start()
    {
        StartCoroutine(ReconcileAfterLoad());
    }

    private IEnumerator ReconcileAfterLoad()
    {
        // Give WorldLoader a frame to finish applying save data
        yield return null;

        // Safety: ensure gameplay isn't unintentionally paused
        if (Time.timeScale == 0f)
        {
            // If no modal (like Tutorial) is active, unpause
            var tutorial = FindObjectOfType<TutorialManager>(true);
            bool tutorialActive = tutorial != null && tutorial.tutorialPanel != null && tutorial.tutorialPanel.activeSelf;
            if (!tutorialActive)
            {
                Time.timeScale = 1f;
                if (gameManager != null) gameManager.isPaused = false;
            }
        }

        // Ensure player control/UI are usable if the wake-up already happened in a previous session
        var wake = FindObjectOfType<WakeUpController>(true);
        bool introWasDone = (wake == null) || !wake.enabled;
        if (!introWasDone && wake != null)
        {
            try { introWasDone = wake.HasWokenUp(); } catch { /* ignore */ }
        }

        if (introWasDone && !SubtitleManager2.IntroStoryComplete)
        {
            SubtitleManager2.ForceIntroComplete();
            fixedIntro = true;
        }

        // Derive hotline state: prefer explicit flag, fall back to phone model inactive
        bool hotlineDone = (hotline != null && hotline.HasCalledHotline()) || (phoneModel != null && !phoneModel.activeInHierarchy);

        if (hotlineDone && !fixedAfterHotline)
        {
            // Remove the "Call 911" gate and move to the next
            SubtitleManager2.CallObjectiveActive = false;
            HandCoverPickup.DoorObjectiveActive = true;

            // If backpack not yet picked, guide to backpack; else guide to door
            bool backpackPicked = (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.IsBackpackUnlocked())
                                  || (backpackModel != null && !backpackModel.activeInHierarchy);

            if (subtitleManager != null)
            {
                if (!backpackPicked)
                {
                    subtitleManager.ShowObjective("Pick up your backpack");
                }
                else
                {
                    subtitleManager.ShowObjective("Exit the bedroom - find a way to open the door safely");
                }
            }

            fixedAfterHotline = true;
        }

        // If the hot door was opened with a towel earlier (explicit static), ensure fuse box objective is available
        if (!fixedAfterDoorWithTowel && HotDoorHandle.DoorOpenedWithTowel)
        {
            FuseBoxInteraction.FuseBoxObjectiveActive = true;

            if (subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(
                    "With all this smoke, I should turn off the electricity first.",
                    3f,
                    () => subtitleManager.ShowObjective("Find and turn off the main breaker")
                );
            }
            fixedAfterDoorWithTowel = true;
        }

        // If all fires are out (derived from particles), ensure the rescue-cat objective is visible
        if (!fixedRescuePush && AllFiresExtinguished())
        {
            if (subtitleManager != null)
            {
                subtitleManager.ShowCustomMessage(
                    "The fire is out! Now I can rescue Mr. Kitty!",
                    2.5f,
                    () => subtitleManager.ShowObjective("Rescue Mr. Kitty in the bedroom")
                );
            }
            fixedRescuePush = true;
        }

        // If Mr. Kitty was already rescued (explicit in script), kick off packing
        if (!fixedPackingStart && mrKitty != null && mrKitty.HasReachedCat())
        {
            if (objectiveManager != null)
                objectiveManager.StartPackingObjective();
            else if (subtitleManager != null)
                subtitleManager.ShowObjective("Collect essential items");

            fixedPackingStart = true;
        }
    }

    private static bool AllFiresExtinguished()
    {
        var fires = Object.FindObjectsOfType<SpreadFire>(true);
        foreach (var f in fires)
        {
            if (f != null && f.IsActive())
                return false;
        }
        return true;
    }
}