using UnityEngine;
using UnityEngine.UI;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Slider oxygenSlider;
    public float maxOxygen = 100f;
    public float normalDrainRate = 1.67f;   // oxygen/sec without towel (60 seconds total)
    public float towelDrainRate = 0.83f;    // oxygen/sec with towel (120 seconds total)

    private float currentOxygen;
    private float drainRate;
    private bool isAlive = true;
    private Movements2 playerMovement; // Reference to movement script

    void Start()
    {
        currentOxygen = maxOxygen;
        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;

            // Force fill color update
            Image fillImage = oxygenSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.blue;
            }
        }

        // Start with fast drain
        drainRate = normalDrainRate;

        // Hide bar at start, SubtitleManager will show it after intro
        if (oxygenSlider != null)
            oxygenSlider.gameObject.SetActive(false);

        // Get reference to Movements script
        playerMovement = GetComponent<Movements2>();

        Debug.Log("PlayerOxygen initialized - Value: " + currentOxygen + "/" + maxOxygen + ", drain rate: " + drainRate);
    }

    void Update()
    {
        if (!isAlive) return;

        // Drain oxygen continuously (even when bar is hidden)
        currentOxygen -= drainRate * Time.deltaTime;
        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // Update slider if it exists (works even when hidden)
        if (oxygenSlider != null)
            oxygenSlider.value = currentOxygen;

        // Oxygen runs out
        if (currentOxygen <= 0f)
        {
            isAlive = false;
            Debug.Log("Player suffocated!");

            // Connect to movement script death system
            if (playerMovement != null)
            {
                playerMovement.TakeDamage(playerMovement.currentHealth); // This will kill the player
            }
        }
    }

    // Called when towel is picked up
    public void EquipTowel()
    {
        drainRate = towelDrainRate;
        Debug.Log("Towel equipped → oxygen drains slower (" + towelDrainRate + "/sec instead of " + normalDrainRate + "/sec)");
    }

    // Called by SubtitleManager when intro finishes
    public void ShowOxygenBar()
    {
        if (oxygenSlider != null)
        {
            oxygenSlider.gameObject.SetActive(true);

            // Force color update when showing
            Image fillImage = oxygenSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = Color.blue;
            }

            Debug.Log("Oxygen bar now visible! Current oxygen: " + currentOxygen.ToString("F1"));
        }
    }

    // Optional: Method to hide oxygen bar
    public void HideOxygenBar()
    {
        if (oxygenSlider != null)
            oxygenSlider.gameObject.SetActive(false);
    }

    // Optional: Get current oxygen percentage for other scripts
    public float GetOxygenPercentage()
    {
        return (currentOxygen / maxOxygen) * 100f;
    }

    // Optional: Check if player is alive
    public bool IsAlive()
    {
        return isAlive;
    }
}