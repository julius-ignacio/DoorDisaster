using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class HeartSys : MonoBehaviour
{
    public Image[] hearts;
    public int currentHearts = 8;
    public bool isHelmetUsed = false;
    public GameNotifier gameNotifier;
    public Sprite yellowHeart;

    void Start()
    {
        // Hide extras at start; load will re-enable if needed
        for (int i = 8; i < hearts.Length; i++)
            hearts[i].gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) { TakeDamage(1); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { HelmetUsed(); }
    }

    // Call from load to set state without SFX or re-running gameplay logic
    public void ApplyHelmetUIState(bool used, int heartsCount)
    {
        isHelmetUsed = used;

        // Toggle extra heart objects (indexes 8+ if present)
        for (int i = 8; i < hearts.Length; i++)
            hearts[i].gameObject.SetActive(used);

        currentHearts = Mathf.Clamp(heartsCount, 0, hearts.Length);
        UpdateHearts();
    }

    public void HelmetUsed()
    {
        isHelmetUsed = true;
        if (currentHearts == 8)
        {
            currentHearts += 5;
            for (int i = 8; i <= 12 && i < hearts.Length; i++)
                hearts[i].gameObject.SetActive(true);
        }
        // ... your existing incremental cases ...

        AudioManager.Instance.PlaySFX(23);
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < currentHearts;
    }

    public void TakeDamage(int amount)
    {
        currentHearts -= amount;
        AudioManager.Instance.PlaySFX(16);
        currentHearts = Mathf.Clamp(currentHearts, 0, hearts.Length);

        if (isHelmetUsed && currentHearts <= 8)
        {
            AudioManager.Instance.PlaySFX(24);
            gameNotifier.HelmetBreak();
            isHelmetUsed = false;
        }

        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHearts = Mathf.Clamp(currentHearts + amount, 0, hearts.Length);
        UpdateHearts();
    }
}