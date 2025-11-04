using UnityEngine;
using UnityEngine.UI;

public class OxygenSys_Water : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Slider oxygenSlider;     // UI slider for oxygen
    public int maxOxygen = 10;      // Maximum oxygen
    public int currentOxygen;       // Current oxygen

    private void Start()
    {
        currentOxygen = maxOxygen;

        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }

        UpdateOxygen();
    }

    public void UpdateOxygen()
    {
        if (oxygenSlider != null)
        {
            oxygenSlider.maxValue = maxOxygen;
            oxygenSlider.value = currentOxygen;
        }
    }

    public void UseOxygen(int amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        UpdateOxygen();
    }

    public void RefillOxygen(int amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        UpdateOxygen();
    }

    public void ResetOxygen()
    {
        currentOxygen = maxOxygen;
        UpdateOxygen();
    }
}
