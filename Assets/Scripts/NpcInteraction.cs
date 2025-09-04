using Narrate;
using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
     public GameObject helpButton; // assign in Inspector
    public InteractiveNarrationTrigger narrationTrigger; // assign your narration trigger

    void Start()
    {
        // Make sure the help button is hidden at the start
        if (helpButton != null)
            helpButton.SetActive(false);

        // Subscribe to narration finished event
        if (narrationTrigger != null && narrationTrigger.OnLineFinish is UnityEngine.Events.UnityEvent onLineFinishEvent)
            onLineFinishEvent.AddListener(ShowHelpButton);
    }

    void ShowHelpButton()
    {
        if (helpButton != null)
            helpButton.SetActive(true);
    }
}
