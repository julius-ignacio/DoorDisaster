using UnityEngine;

public class IntroTrigger_Water : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject introPanel; // Assign your Intro_Panel_Game_Guide here

    private bool hasShown = false;

    private void Start()
    {
        // Always hide the intro panel at the start
        if (introPanel != null)
            introPanel.SetActive(false);

        // Check if it has been shown before (saved)
        if (PlayerPrefs.GetInt("IntroShown", 0) == 1)
        {
            hasShown = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only show if not shown before and collider is the player
        if (!hasShown && other.CompareTag("Player"))
        {
            introPanel.SetActive(true);
            Time.timeScale = 0f; // Optional: pause game
        }
    }

    // Call this from your "Next" or "Close" button
    public void CloseIntro()
    {
        introPanel.SetActive(false);
        Time.timeScale = 1f;
        hasShown = true;
        PlayerPrefs.SetInt("IntroShown", 1); // Remember it’s shown
    }
}
