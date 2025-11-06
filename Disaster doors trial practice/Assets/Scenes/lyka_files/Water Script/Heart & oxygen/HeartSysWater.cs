using UnityEngine;
using UnityEngine.UI;

public class HeartSysWater : MonoBehaviour
{
    [Header("Heart Settings")]
    public Image[] fullHearts;   // Array of UI Images that represent full (active) hearts showing player health
    public Image[] emptyHearts;  // Array of UI Images that represent empty (inactive) hearts showing lost health

    public int currentHearts;    // Current number of hearts (player's current health)
    public int maxHearts;        // Maximum number of hearts (total health capacity)

    private void Start()
    {
        // Initialize the system at the start of the game
        maxHearts = fullHearts.Length;  // The maximum hearts is equal to how many full heart images are assigned in the inspector
        currentHearts = maxHearts;      // Player starts with full health
        UpdateHearts();                 // Update the visual UI to show all full hearts
    }

    public void UpdateHearts()
    {
        // This method visually updates the heart icons depending on current health
        for (int i = 0; i < maxHearts; i++)
        {
            if (i < currentHearts)
            {
                // If the current index is less than the number of current hearts,
                // that means this heart is still "active" (full red heart)
                fullHearts[i].enabled = true;   // Show the full heart
                emptyHearts[i].enabled = false; // Hide the empty heart at this position
            }
            else
            {
                // Otherwise, this heart is considered "lost" or "damaged"
                fullHearts[i].enabled = false;  // Hide the full heart
                emptyHearts[i].enabled = true;  // Show the empty heart (greyed out)
            }
        }
    }

    // ❤️ Lose hearts (damage)
    public void UseHeart(int amount)
    {
        // Reduce the player's current hearts by the specified amount
        currentHearts -= amount;

        // Ensure the value doesn't go below 0 or above the maxHearts
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        // Update the heart UI after taking damage
        UpdateHearts();
    }

    // ❤️ Heal hearts (restore health)
    public void RefillHeart(int amount)
    {
        // Add a certain amount of hearts to restore health
        currentHearts += amount;

        // Prevent going above the maximum hearts or below zero
        currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts);

        // Update the UI to reflect the healed hearts
        UpdateHearts();
    }

    // ✅ Alias for compatibility with InventoryManager_Water
    public void AddHeart(int amount)
    {
        // This method simply calls RefillHeart() to make the code compatible
        // with other scripts that might call AddHeart instead of RefillHeart
        RefillHeart(amount);
    }

    // ♻️ Fully restore all hearts
    public void ResetHearts()
    {
        // This method restores the player's hearts to full health instantly
        currentHearts = maxHearts;

        // Update the UI to show all full hearts
        UpdateHearts();
    }
}
