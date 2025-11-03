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
    public Image panickedStateImage; // 👈 Assign in Inspector

    private float currentOxygen;
    private float drainRate;
    private bool isAlive = true;
    private Movements2 playerMovement;

    private bool oxygenDrainActive = false;
    private bool hasTriggeredDeath = false;

    public static bool InHallwayChase { get; set; } = false;

    void Start()
    {
        currentOxygen = maxOxygen;

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
            panickedStateImage.gameObject.SetActive(false); // Hide panic UI at start
        }

        drainRate = normalDrainRate;
        playerMovement = GetComponent<Movements2>();

        Debug.Log("PlayerOxygen initialized - Value: " + currentOxygen + "/" + maxOxygen + ", drain rate: " + drainRate);
        Debug.Log("Oxygen drain PAUSED until intro story completes");
    }

    void Update()
    {
        if (!isAlive) return;

        if (!oxygenDrainActive)
        {
            if (SubtitleManager2.IntroStoryComplete)
            {
                oxygenDrainActive = true;
                Debug.Log("Intro story complete - oxygen drain ACTIVATED");
            }
            else
            {
                return;
            }
        }

        if (InHallwayChase)
        {
            drainRate = hallwayDrainRate;
        }

        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        // ✅ Show panic UI if oxygen is low
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
        Debug.Log("Towel equipped → oxygen drains slower (" + towelDrainRate + "/sec instead of " + normalDrainRate + "/sec)");
    }

    public void RefillOxygen()
    {
        currentOxygen = maxOxygen;
        hasTriggeredDeath = false;
        isAlive = true;

        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        if (panickedStateImage != null)
            panickedStateImage.gameObject.SetActive(false); // Hide panic UI on refill

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

    public float GetOxygenPercentage()
    {
        return (currentOxygen / maxOxygen) * 100f;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
