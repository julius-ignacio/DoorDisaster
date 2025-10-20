using UnityEngine;
using UnityEngine.UI;

public class OxygenSys_Water : MonoBehaviour
{
    [Header("Oxygen Settings")]
    public Image[] oxygenBars;   // Blue bubbles
    public Image[] deadOxygen;   // Red bubbles

    public int currentOxygen;
    public int maxOxygen;

    private void Start()
    {
        maxOxygen = oxygenBars.Length;
        currentOxygen = maxOxygen;
        UpdateOxygen();
    }

    public void UpdateOxygen()
    {
        for (int i = 0; i < maxOxygen; i++)
        {
            if (i < currentOxygen)
            {
                // If oxygen is still available
                oxygenBars[i].enabled = true;
                deadOxygen[i].enabled = false;
            }
            else
            {
                // Oxygen lost, show dead bubble
                oxygenBars[i].enabled = false;
                deadOxygen[i].enabled = true;
            }
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
}
