using UnityEngine;
using UnityEngine.UI;

public class HeartSysWater : MonoBehaviour
{
    [Header("Heart Settings")]
    public Image[] fullHearts;   // Full red hearts (healthy)
    public Image[] emptyHearts;  // Empty/grey hearts (lost health)

    public int currentHearts;
    public int maxHearts;

    private void Start()
    {
        maxHearts = fullHearts.Length;
        currentHearts = maxHearts;
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < maxHearts; i++)
        {
            if (i < currentHearts)
            {
                // Heart is full
                fullHearts[i].enabled = true;
                emptyHearts[i].enabled = false;
            }
            else
            {
                // Heart lost
                fullHearts[i].enabled = false;
                emptyHearts[i].enabled = true;
            }
        }
    }

    public void UseHeart(int amount)
    {
        currentHearts -= amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        UpdateHearts();
    }

    public void RefillHeart(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        UpdateHearts();
    }

    public void ResetHearts()
    {
        currentHearts = maxHearts;
        UpdateHearts();
    }
}
