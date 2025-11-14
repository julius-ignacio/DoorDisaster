using System.Collections;
using UnityEngine;

// Runs after WorldLoader, reconciles objectives with actual world state
[DefaultExecutionOrder(1200)]
public class SequenceManager : MonoBehaviour
{
    [Header("Optional references (auto-find if empty)")]
    public SubtitleManager2 subtitleManager;
    public ObjectiveManager objectiveManager;
    public EmergencyHotlineCall hotline;
    public MrKittyPickup mrKitty;

    [Header("Optional world models (for fallback state checks)")]
    public GameObject phoneModel;
    public GameObject backpackModel;

    // Guards to avoid duplicate prompts
    private bool fixedIntro;
    private bool fixedAfterHotline;
    private bool fixedAfterDoorWithTowel;
    private bool fixedAfterFuseBox;
    private bool fixedRescuePush;
    private bool fixedPackingStart;
    private bool fixedAfterEssentials;

    private void Awake()
    {
        if (subtitleManager == null) subtitleManager = FindObjectOfType<SubtitleManager2>(true);
        if (objectiveManager == null) objectiveManager = FindObjectOfType<ObjectiveManager>(true);
        if (hotline == null) hotline = FindObjectOfType<EmergencyHotlineCall>(true);
        if (mrKitty == null) mrKitty = FindObjectOfType<MrKittyPickup>(true);
    }

    private void Start()
    {
        StartCoroutine(ReconcileAfterLoad());
    }

    private IEnumerator ReconcileAfterLoad()
    {
        // Wait for everything to load
        yield return null;
        yield return null;

        // Declare player once at the top
        var player = FindObjectOfType<Movements2>(true);

        // ✅ NEW: If no save data exists, skip reconciliation (fresh start)
        var dm = DataManager.Instance;
        if (dm != null && !WorldSaveSystem.HasSaveData(dm.currentTrial, dm.currentMode))
        {
            Debug.Log("🆕 SequenceManager: Fresh start detected - skipping reconciliation, letting game start naturally");

            // Still ensure player can move and UI is visible
            if (player != null && !player.enabled)
            {
                player.enabled = true;
                var cc = player.GetComponent<CharacterController>();
                if (cc != null && !cc.enabled)
                    cc.enabled = true;
            }

            if (subtitleManager != null)
            {
                if (subtitleManager.healthBar != null)
                    subtitleManager.healthBar.SetActive(true);

                if (subtitleManager.oxygenBar != null)
                    subtitleManager.oxygenBar.SetActive(true);

                var oxygenSystem = FindObjectOfType<PlayerOxygen>(true);
                if (oxygenSystem != null)
                    oxygenSystem.ShowOxygenBar();
            }

            yield break; // Exit early - don't reconcile anything
        }

        Debug.Log("📂 SequenceManager: Save data detected - reconciling state");

        // ✅ CRITICAL: Tell LockedDoor to restore its state AFTER static flags are loaded
        var lockedDoor = FindObjectOfType<LockedDoor>();
        if (lockedDoor != null)
        {
            lockedDoor.RestoreState();
            Debug.Log("🔑 SequenceManager: Called LockedDoor.RestoreState()");
        }

        // Ensure gameplay isn't paused
        if (Time.timeScale == 0f)
        {
            var tutorial = FindObjectOfType<TutorialManager>(true);
            bool tutorialActive = tutorial != null && tutorial.tutorialPanel != null && tutorial.tutorialPanel.activeSelf;
            if (!tutorialActive)
            {
                Time.timeScale = 1f;
                if (GameManager.Instance != null) GameManager.Instance.isPaused = false;
            }
        }

        // Force UI visible
        if (subtitleManager != null)
        {
            if (subtitleManager.healthBar != null)
                subtitleManager.healthBar.SetActive(true);

            if (subtitleManager.oxygenBar != null)
                subtitleManager.oxygenBar.SetActive(true);

            var oxygenSystem = FindObjectOfType<PlayerOxygen>(true);
            if (oxygenSystem != null)
                oxygenSystem.ShowOxygenBar();
        }

        // Ensure player can move (reuse the player variable declared at the top)
        if (player != null && !player.enabled)
        {
            player.enabled = true;
            var cc = player.GetComponent<CharacterController>();
            if (cc != null && !cc.enabled)
                cc.enabled = true;
        }

        // Check wake-up status
        var wake = FindObjectOfType<WakeUpController>(true);
        bool introWasDone = (wake == null) || !wake.enabled;
        if (wake != null && wake.enabled)
        {
            try { introWasDone = wake.HasWokenUp(); } catch { }
        }

        if (introWasDone && !SubtitleManager2.IntroStoryComplete)
        {
            SubtitleManager2.ForceIntroComplete();
            fixedIntro = true;
        }

        // === PHASE DETECTION ===
        bool hotlineDone = CheckHotlineStatus();
        bool backpackPicked = CheckBackpackStatus();
        bool clothPicked = CheckClothStatus();
        bool doorOpened = HotDoorHandle.DoorOpenedWithTowel;
        bool fuseBoxOff = CheckFuseBoxStatus();
        bool wetTowelPicked = CheckWetTowelStatus();
        bool fireExtinguisherUsed = CheckFireExtinguisherStatus();
        bool catRescued = CheckCatStatus();
        bool essentialsCollected = CheckEssentialsStatus();
        bool frontDoorBlocked = CheckFrontDoorStatus();
        bool windowBroken = CheckWindowStatus();
        bool windowTried = false; // Track if player tried the jammed window
        bool hasHeavyObject = false; // Track if player has heavy object

        var windowEscape = FindObjectOfType<WindowEscape>();
        if (windowEscape != null)
        {
            try
            {
                windowTried = windowEscape.HasTriedWindow();
                hasHeavyObject = windowEscape.HasHeavyObject();
            }
            catch { }
        }
        bool doorUnlocked = CheckDoorStatus();

        Debug.Log($"🔍 PHASE CHECK:");
        Debug.Log($"  Phase 1: Hotline={hotlineDone}, Backpack={backpackPicked}, Cloth={clothPicked}, Door={doorOpened}, FuseBox={fuseBoxOff}");
        Debug.Log($"  Phase 2: WetTowel={wetTowelPicked}, Extinguisher={fireExtinguisherUsed}, DoorUnlocked={doorUnlocked}, Cat={catRescued}");
        Debug.Log($"  Phase 3: Essentials={essentialsCollected}, FrontDoor={frontDoorBlocked}, WindowTried={windowTried}, HeavyObject={hasHeavyObject}, InHallway={windowBroken}");
        Debug.Log($"  Special: SDR={SDRTrigger.SDRTriggered}, DoorFire={DoorFireTrigger.FireMessageShown}");

        // === SHOW CORRECT OBJECTIVE ===
        if (subtitleManager != null)
        {
            // ✅ PRIORITY 1: CHECK SDR FIRST (highest priority for Phase 3)
            if (SDRTrigger.SDRTriggered)
            {
                Debug.Log("📋 PHASE 3 (SDR): Try bedroom window");
                subtitleManager.ShowObjective("Try the window in the bedroom");
            }
            // ✅ PRIORITY 2: CHECK DOOR FIRE SECOND (only if SDR hasn't happened yet)
            else if (DoorFireTrigger.FireMessageShown && !SDRTrigger.SDRTriggered)
            {
                Debug.Log("📋 PHASE 3 (Fire): Find alternative escape");
                subtitleManager.ShowObjective("Find an alternative escape route - try the window!");
            }
            // PHASE 4: Final Escape (Hallway)
            else if (windowBroken && PlayerOxygen.InHallwayChase)
            {
                Debug.Log("📋 PHASE 4: Final escape through hallway");
                subtitleManager.ShowObjective("Reach the exit at the end of the hallway");
            }
            // PHASE 3: Escape Attempt (detailed progression)
            else if (catRescued && essentialsCollected && hasHeavyObject)
            {
                Debug.Log("📋 PHASE 3: Break window with heavy object");
                subtitleManager.ShowObjective("Use the heavy object to break the bedroom window");
            }
            else if (catRescued && essentialsCollected && windowTried)
            {
                Debug.Log("📋 PHASE 3: Window jammed - find heavy object");
                subtitleManager.ShowObjective("The window is jammed - find a heavy object to break it");
            }
            else if (catRescued && essentialsCollected && frontDoorBlocked)
            {
                Debug.Log("📋 PHASE 3: Front door blocked - try bedroom window");
                subtitleManager.ShowObjective("The front door is blocked - try the bedroom window");
            }
            else if (catRescued && essentialsCollected)
            {
                Debug.Log("📋 PHASE 3: Try front door");
                subtitleManager.ShowObjective("Find the nearest exit and escape the fire");
            }
            else if (catRescued)
            {
                Debug.Log("📋 PHASE 3: Collect essentials");
                if (objectiveManager != null && !fixedPackingStart)
                {
                    objectiveManager.StartPackingObjective();
                    fixedPackingStart = true;
                }
                else
                {
                    subtitleManager.ShowObjective("Collect essential items");
                }
            }
            // PHASE 2: Cat Rescue
            else if (fuseBoxOff && fireExtinguisherUsed && doorUnlocked)
            {
                Debug.Log("📋 PHASE 2: Rescue Mr. Kitty (door unlocked)");
                subtitleManager.ShowObjective("Rescue Mr. Kitty!");
                fixedRescuePush = true;
            }
            else if (fuseBoxOff && fireExtinguisherUsed && !doorUnlocked)
            {
                Debug.Log("📋 PHASE 2: Unlock bedroom door");
                subtitleManager.ShowObjective("Find the key and unlock the bedroom door");
            }
            else if (fuseBoxOff && wetTowelPicked)
            {
                Debug.Log("📋 PHASE 2: Use fire extinguisher");
                subtitleManager.ShowObjective("Find the fire extinguisher and clear the path");
            }
            else if (fuseBoxOff)
            {
                Debug.Log("📋 PHASE 2: Get wet towel");
                subtitleManager.ShowObjective("Find a wet towel for protection");
            }
            // PHASE 1: Initial Response (Bedroom)
            else if (doorOpened)
            {
                Debug.Log("📋 PHASE 1: Turn off fuse box");
                FuseBoxInteraction.FuseBoxObjectiveActive = true;
                subtitleManager.ShowObjective("Find and turn off the main breaker");
                fixedAfterDoorWithTowel = true;
            }
            else if (clothPicked && backpackPicked)
            {
                Debug.Log("📋 PHASE 1: Use cloth on door");
                HandCoverPickup.DoorObjectiveActive = true;

                if (HotDoorHandle.touchedHotHandle)
                {
                    subtitleManager.ShowObjective("Use the cloth to open the bedroom door");
                }
                else
                {
                    subtitleManager.ShowObjective("Go to the bedroom door");
                }
            }
            else if (backpackPicked)
            {
                Debug.Log("📋 PHASE 1: Find cloth to exit bedroom");
                HandCoverPickup.DoorObjectiveActive = true;
                subtitleManager.ShowObjective("Exit the bedroom - find a way to open the door safely");
                fixedAfterHotline = true;
            }
            else if (hotlineDone)
            {
                Debug.Log("📋 PHASE 1: Pick up backpack");
                SubtitleManager2.CallObjectiveActive = false;
                HandCoverPickup.DoorObjectiveActive = true;
                subtitleManager.ShowObjective("Pick up your backpack");
                fixedAfterHotline = true;
            }
            else if (SubtitleManager2.IntroStoryComplete)
            {
                Debug.Log("📋 PHASE 1: Call 911");
                SubtitleManager2.CallObjectiveActive = true;
                subtitleManager.ShowObjective("Find the phone and call for help!");
            }
        }
    }

    // ========== PHASE 1 CHECKS ==========
    private bool CheckHotlineStatus()
    {
        var hotlineScripts = FindObjectsOfType<EmergencyHotlineCall>(true);
        if (hotlineScripts.Length > 0)
        {
            try
            {
                if (hotlineScripts[0].HasCalledHotline())
                    return true;
            }
            catch { }
        }

        if (SubtitleManager2.IntroStoryComplete && !SubtitleManager2.CallObjectiveActive)
            return true;

        return false;
    }

    private bool CheckBackpackStatus()
    {
        if (InventoryManager_fire.Instance != null && InventoryManager_fire.Instance.IsBackpackUnlocked())
            return true;

        if (backpackModel != null && !backpackModel.activeInHierarchy)
            return true;

        var backpackObj = GameObject.Find("Backpack");
        if (backpackObj != null && !backpackObj.activeInHierarchy)
            return true;

        return false;
    }

    private bool CheckClothStatus()
    {
        var clothPickup = FindObjectOfType<HandCoverPickup>();
        if (clothPickup != null && clothPickup.HasBeenPickedUp())
            return true;

        string[] clothNames = { "Cloth", "HandCover", "Towel", "ClothObject", "HandCloth" };
        foreach (var name in clothNames)
        {
            var obj = GameObject.Find(name);
            if (obj != null && !obj.activeInHierarchy)
                return true;
        }

        return false;
    }

    private bool CheckFuseBoxStatus()
    {
        if (BreakerPuzzle.BreakerPuzzleComplete)
            return true;

        var fuseBox = FindObjectOfType<FuseBoxInteraction>();
        if (fuseBox != null)
        {
            try
            {
                if (fuseBox.IsTurnedOff())
                    return true;
            }
            catch { }
        }

        return false;
    }

    // ========== PHASE 2 CHECKS ==========
    private bool CheckWetTowelStatus()
    {
        if (TowelPickup.HasTeleportedToHouseB)
            return true;

        var towelPickup = FindObjectOfType<TowelPickup>();
        if (towelPickup != null && towelPickup.HasPickedUpTowel())
            return true;

        var wetTowelObj = GameObject.Find("WetTowel");
        if (wetTowelObj != null && !wetTowelObj.activeInHierarchy)
            return true;

        return false;
    }

    private bool CheckFireExtinguisherStatus()
    {
        if (FireExtinguisher.AllFiresOut)
            return true;

        return AllFiresExtinguished();
    }

    private bool CheckDoorStatus()
    {
        return LockedDoor.DoorUnlocked;
    }

    private bool CheckCatStatus()
    {
        if (MrKittyPickup.CatRescued)
            return true;

        var catObj = GameObject.Find("MrKitty") ?? GameObject.Find("Cat");
        if (catObj != null && !catObj.activeInHierarchy)
            return true;

        if (mrKitty != null && mrKitty.HasReachedCat())
            return true;

        return false;
    }

    // ========== PHASE 3 CHECKS ==========
    private bool CheckEssentialsStatus()
    {
        if (objectiveManager != null)
        {
            try
            {
                return objectiveManager.GetObjectiveStage() >= 2;
            }
            catch { }
        }

        return false;
    }

    private bool CheckFrontDoorStatus()
    {
        var doorTrigger = FindObjectOfType<DoorFireTrigger>();
        if (doorTrigger != null && doorTrigger.HasShownFireMessage())
            return true;

        return false;
    }

    private bool CheckWindowStatus()
    {
        var windowEscape = FindObjectOfType<WindowEscape>();
        if (windowEscape != null && windowEscape.HasHeavyObject())
            return true;

        if (PlayerOxygen.InHallwayChase)
            return true;

        return false;
    }

    // ========== HELPER ==========
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