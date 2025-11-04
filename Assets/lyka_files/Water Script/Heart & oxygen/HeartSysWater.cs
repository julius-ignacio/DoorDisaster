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
    if (fullHearts == null || emptyHearts == null)
    {
        Debug.LogWarning("HeartSysWater: Heart arrays not assigned!");
        return;
    }

    for (int i = 0; i < maxHearts; i++)
    {
        if (i >= fullHearts.Length || i >= emptyHearts.Length)
            break;

        if (fullHearts[i] == null || emptyHearts[i] == null)
            continue;

        bool isFull = i < currentHearts;
        fullHearts[i].enabled = isFull;
        emptyHearts[i].enabled = !isFull;
    }
}


    // ❤️ Lose hearts (damage)
    public void UseHeart(int amount)
    {
        currentHearts -= amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        UpdateHearts();
    }

    // ❤️ Heal hearts (restore health)
    public void RefillHeart(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);
        UpdateHearts();
    }

    // ✅ Alias for compatibility with InventoryManager_Water
    public void AddHeart(int amount)
    {
        RefillHeart(amount);
    }

    // ♻️ Fully restore all hearts
    public void ResetHearts()
    {
        currentHearts = maxHearts;
        UpdateHearts();
    }
}
