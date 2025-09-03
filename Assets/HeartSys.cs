using UnityEngine;
using UnityEngine.UI;

public class HeartSys : MonoBehaviour
{
    public Image[] hearts; // assign 3 heart images in inspector
    public int currentHearts = 3;

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentHearts;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHearts -= amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, hearts.Length);
        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Clamp(currentHearts, 0, hearts.Length);
        UpdateHearts();
    }
}
