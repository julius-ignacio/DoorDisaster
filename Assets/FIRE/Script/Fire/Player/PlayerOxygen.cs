using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Slider oxygenSlider;
    public float maxOxygen = 100f;
    public float normalDrainRate = 0.8f;
    public float towelDrainRate = 0.3f;
    public float hallwayDrainRate = 1f;

    [Header("Panic UI")]
    public Image panickedStateImage;

    private float currentOxygen;
    private float drainRate;
    private bool isAlive = true;
    private Movements2 playerMovement;

    private bool oxygenDrainActive = false;
    private bool hasTriggeredDeath = false;

    public static bool InHallwayChase { get; set; } = false;

    // ✅ Static properties for save system
    private static float savedOxygenLevel = -1f; // -1 means not set
    private static bool savedIsTowelEquipped = false;

    void Start()
    {
        // ✅ Restore oxygen level if available
        if (savedOxygenLevel >= 0f)
        {
            currentOxygen = savedOxygenLevel;
            Debug.Log($"📂 Restored oxygen level: {currentOxygen}");
        }
        else
        {
            currentOxygen = maxOxygen;
        }

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;

            Image fillImage = oxygenSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.blue;
            }

            oxygenSlider.gameObject.SetActive(false);
        }

        if (panickedStateImage != null)
        {
            panickedStateImage.gameObject.SetActive(false);
        }

        // ✅ Restore towel state if equipped
        if (savedIsTowelEquipped)
        {
            drainRate = towelDrainRate;
            Debug.Log($"📂 Restored towel equipped state: drain rate = {drainRate}");
        }
        else
        {
            drainRate = normalDrainRate;
        }

        playerMovement = GetComponent<Movements2>();

        Debug.Log("PlayerOxygen initialized - Value: " + currentOxygen + "/" + maxOxygen + ", drain rate: " + drainRate);
        Debug.Log("Oxygen drain PAUSED until intro story completes");
    }

    void Update()
    {
        if (!isAlive) return;

        // ✅ Check if oxygen drain should be active (once intro is complete, it stays active)
        if (!oxygenDrainActive)
        {
            if (SubtitleManager2.IntroStoryComplete)
            {
                oxygenDrainActive = true;
                Debug.Log("Intro story complete - oxygen drain ACTIVATED");
            }
            else
            {
                return; // Don't drain oxygen until intro is done
            }
        }

        // ✅ Oxygen drains regardless of UI visibility
        if (InHallwayChase)
        {
            drainRate = hallwayDrainRate;
        }
        else if (savedIsTowelEquipped)
        {
            drainRate = towelDrainRate;
        }
        else
        {
            drainRate = normalDrainRate;
        }

        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // ✅ Save oxygen level continuously
        savedOxygenLevel = currentOxygen;

        // ✅ Update slider only if it exists and is active
        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        // Show panic UI if oxygen is low
        if (panickedStateImage != null)
        {
            bool shouldShowPanic = currentOxygen <= 30f;
            if (panickedStateImage.gameObject.activeSelf != shouldShowPanic)
            {
                panickedStateImage.gameObject.SetActive(shouldShowPanic);
                Debug.Log("PanickedState UI " + (shouldShowPanic ? "activated!" : "hidden."));
            }
        }

        if (currentOxygen <= 0f && !hasTriggeredDeath)
        {
            hasTriggeredDeath = true;
            isAlive = false;
            Debug.Log("Player suffocated!");

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
                CharacterController controller = playerMovement.GetComponent<CharacterController>();
                if (controller != null)
                    controller.enabled = false;
            }

            GameOverManager.TriggerDeath("OUT OF OXYGEN", "You ran out of breathable air.");
        }
    }

    public void EquipTowel()
    {
        drainRate = towelDrainRate;
        savedIsTowelEquipped = true; // ✅ Save towel state
        Debug.Log("Towel equipped → oxygen drains slower (" + towelDrainRate + "/sec instead of " + normalDrainRate + "/sec)");
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        savedOxygenLevel = currentOxygen; // ✅ Save refilled state
        hasTriggeredDeath = false;
        isAlive = true;

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        if (panickedStateImage != null)
            panickedStateImage.gameObject.SetActive(false);

        Debug.Log("Oxygen refilled to max! (" + maxOxygen + ")");
    }

    public void ShowOxygenBar()
    {
        if (oxygenSlider != null)
        {
            oxygenSlider.gameObject.SetActive(true);
            Image fillImage = oxygenSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.blue;
            }
            Debug.Log("Oxygen bar now visible! Current oxygen: " + currentOxygen.ToString("F1"));
        }
    }

    public void HideOxygenBar()
    {
        if (oxygenSlider != null)
            oxygenSlider.gameObject.SetActive(false);
    }

    // ✅ NEW: Public method to ensure oxygen drain is active
    public void EnsureOxygenDrainActive()
    {
        if (SubtitleManager2.IntroStoryComplete && !oxygenDrainActive)
        {
            oxygenDrainActive = true;
            Debug.Log("⚠️ Oxygen drain manually re-enabled");
        }
    }

    public float GetOxygenPercentage()
    {
        return (currentOxygen / maxOxygen) * 100f;
    }

    public bool IsAlive()
    {
        return isAlive;
    }

    // ✅ Public getter for current oxygen
    public float GetCurrentOxygen()
    {
        return currentOxygen;
    }

    // ✅ Static methods for save system
    public static void RestoreOxygenState(float oxygen, bool towelEquipped)
    {
        savedOxygenLevel = oxygen;
        savedIsTowelEquipped = towelEquipped;
        Debug.Log($"📂 Restored oxygen state: oxygen={oxygen}, towel={towelEquipped}");
    }

    public static void ResetOxygenProgress()
    {
        savedOxygenLevel = -1f;
        savedIsTowelEquipped = false;
        Debug.Log("🔄 Reset oxygen progress");
    }

    public static float GetSavedOxygenLevel()
    {
        return savedOxygenLevel;
    }

    public static bool GetSavedTowelEquipped()
    {
        return savedIsTowelEquipped;
    }
}