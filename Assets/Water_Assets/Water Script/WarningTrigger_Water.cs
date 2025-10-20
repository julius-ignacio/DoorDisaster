using UnityEngine;
using TMPro;

public class WarningTrigger_Water : MonoBehaviour
{
    [Header("Warning Text Reference")]
    public TextMeshProUGUI warningText;   // Drag your existing Text (TMP) here

    [Header("Display Settings")]
    [TextArea]
    public string warningMessage = "Warning... a massive storm is approaching. Flooding is expected in nearby areas. Seek shelter immediately.";
    public float displayTime = 4f;        // Duration before it hides

    private bool hasShown = false;

    private void Start()
    {
        // Make sure warning text is hidden initially
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger once when the player enters the collider
        if (other.CompareTag("Player") && !hasShown)
        {
            hasShown = true;
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        if (warningText != null)
        {
            warningText.text = warningMessage;
            warningText.gameObject.SetActive(true);

            // Hide text after displayTime seconds
            Invoke(nameof(HideWarning), displayTime);
        }
    }

    void HideWarning()
    {
        if (warningText != null)
            warningText.gameObject.SetActive(false);
    }
}
