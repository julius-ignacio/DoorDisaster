using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays DataManager stats on a Canvas Text component.
/// Attach to a GameObject with a Text or TextMeshProUGUI component.
/// </summary>
public class DataDisplayUI_Water : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag your Text or TextMeshProUGUI component here")]
    public Text legacyText;
    public TextMeshProUGUI tmpText;

    [Header("Update Settings")]
    [Tooltip("How often to refresh the display (in seconds)")]
    public float updateInterval = 0.5f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (DataManager_Water.Instance == null)
        {
            SetText("DataManager not found!");
            return;
        }

        DataManager_Water dm = DataManager_Water.Instance;

        string displayText = $"=== PLAYER STATS ===\n\n" +
                             $"Total Score: {dm.PlayerTotalScore}\n" +
                             $"Quiz Score: {dm.quizScore}\n" +
                             $"Facts Discovered: {dm.factsDiscovered}\n\n" +
                             $"Questions Answered: {dm.totalQuestionsAnswered}\n" + // ✅ fixed lowercase
                             $"Wrong Answers: {dm.wrongAnswers}\n\n" +
                             $"Items Collected: {dm.collectedItems.Count}\n" +
                             $"Keys Collected: {dm.collectedKeys.Count}\n\n" +
                             $"Play Time: {FormatTime(dm.totalPlayTime)}\n" +
                             $"Current Scene: {dm.currentSceneName}";

        SetText(displayText);
    }


    private void SetText(string text)
    {
        if (tmpText != null)
            tmpText.text = text;
        else if (legacyText != null)
            legacyText.text = text;
    }

    private string FormatTime(float seconds)
    {
        int hours = (int)(seconds / 3600);
        int minutes = (int)((seconds % 3600) / 60);
        int secs = (int)(seconds % 60);

        if (hours > 0)
            return $"{hours}h {minutes}m {secs}s";
        else if (minutes > 0)
            return $"{minutes}m {secs}s";
        else
            return $"{secs}s";
    }
}