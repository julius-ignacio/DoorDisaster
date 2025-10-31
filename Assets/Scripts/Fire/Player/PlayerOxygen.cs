using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Slider oxygenSlider;
    public float maxOxygen = 100f;
    public float normalDrainRate = 0.8f;
    public float towelDrainRate = 0.3f;
    public float hallwayDrainRate = 1f; // ✅ Faster drain during hallway chase!

    private float currentOxygen;
    private float drainRate;
    private bool isAlive = true;
    private Movements2 playerMovement;

    // Flags to control when oxygen starts draining
    private bool oxygenDrainActive = false;

    // ✅ Public flag for hallway chase mode
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
        }

        drainRate = normalDrainRate;

        // Hide bar at start, SubtitleManager will show it after intro
        if (oxygenSlider != null)
            oxygenSlider.gameObject.SetActive(false);

        playerMovement = GetComponent<Movements2>();

        Debug.Log("PlayerOxygen initialized - Value: " + currentOxygen + "/" + maxOxygen + ", drain rate: " + drainRate);
        Debug.Log("Oxygen drain PAUSED until intro story completes");
    }

    void Update()
    {
        if (!isAlive) return;

        // Only drain oxygen if intro story is complete
        if (!oxygenDrainActive)
        {
            // Check if story is complete to start draining
            if (SubtitleManager2.IntroStoryComplete)
            {
                oxygenDrainActive = true;
                Debug.Log("Intro story complete - oxygen drain ACTIVATED");
            }
            else
            {
                return; // Don't drain oxygen yet
            }
        }

        // ✅ Check if in hallway chase mode - use faster drain rate
        if (InHallwayChase)
        {
            drainRate = hallwayDrainRate;
        }

        // Drain oxygen continuously
        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // Update slider
        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        // Oxygen runs out
        if (currentOxygen <= 0f)
        {
            isAlive = false;
            Debug.Log("Player suffocated!");

            if (playerMovement != null)
            {
                playerMovement.TakeDamage(playerMovement.currentHealth);
            }
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
        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;
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