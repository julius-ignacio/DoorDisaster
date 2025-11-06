using UnityEngine;
using UnityEngine.UI;

public class OxygenSys_Water : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Slider oxygenSlider;     // UI Slider component that visually represents the player's oxygen level in the UI
    public int maxOxygen = 10;      // The maximum amount of oxygen the player can have (full tank or full breath)
    public int currentOxygen;       // The current amount of oxygen the player currently has left

    private void Start()
    {
        // Initialize oxygen values at the start of the game or scene
        currentOxygen = maxOxygen;  // Player starts with full oxygen

        // Check if the UI slider is assigned in the Inspector
        if (oxygenSlider != null)
        {
            // Set the maximum value of the slider to match the maximum oxygen amount
            oxygenSlider.maxValue = maxOxygen;

            // Set the current slider value to match the current oxygen amount (starts full)
            oxygenSlider.value = currentOxygen;
        }

        // Update the slider visually so it matches the current oxygen level
        UpdateOxygen();
    }

    public void UpdateOxygen()
    {
        // This function refreshes the UI slider whenever oxygen changes
        if (oxygenSlider != null)
        {
            // Ensure the slider displays correct oxygen info:
            // - Its maximum possible value (maxOxygen)
            // - Its current fill level (currentOxygen)
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }
    }

    public void UseOxygen(int amount)
    {
        // This method is called when the player loses oxygen (e.g., staying underwater)
        currentOxygen -= amount; // Subtract the amount used

        // Prevent the value from going below 0 or above the maximum
        // Mathf.Clamp keeps currentOxygen between 0 and maxOxygen
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        // Update the slider UI to show the reduced oxygen level
        UpdateOxygen();
    }

    public void RefillOxygen(int amount)
    {
        // This method is called when the player gains oxygen (e.g., reaching air or using an oxygen tank)
        currentOxygen += amount; // Add the amount restored

        // Keep the value within 0 and the maximum range
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);

        // Update the slider UI to show the new (increased) oxygen level
        UpdateOxygen();
    }

    public void ResetOxygen()
    {
        // This method fully restores the player's oxygen back to maximum
        currentOxygen = maxOxygen;

        // Update the slider UI to display a full bar again
        UpdateOxygen();
    }
}
