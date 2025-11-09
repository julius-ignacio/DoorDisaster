using UnityEngine;

public class TutorialAndRadioManager : MonoBehaviour
{
    [Header("References")]
    public TutorialManager_Water tutorialManager; // Reference to your tutorial system
    public RadioIntro_Water radioIntro;           // Reference to the radio intro script

    private bool radioStarted = false;

    void Start()
    {
        // Make sure the radio doesn't start automatically
        if (radioIntro != null)
            radioIntro.enabled = false;
    }

    void Update()
    {
        // Wait for tutorial to finish before starting radio
        if (!radioStarted && tutorialManager != null && !tutorialManager.tutorialPanel.activeSelf)
        {
            StartRadioIntro();
        }
    }

    void StartRadioIntro()
    {
        radioStarted = true;

        if (radioIntro != null)
        {
            radioIntro.enabled = true; // This starts the radio sequence since its Start() runs on enable
            Debug.Log("🎧 Radio intro started after tutorial!");
        }
    }
}
