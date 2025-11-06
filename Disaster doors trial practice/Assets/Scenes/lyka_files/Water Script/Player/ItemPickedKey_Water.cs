using UnityEngine;
using TMPro;
using System.Collections;

public class ItemPickedKey_Water : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject keyPickedUI; // panel that shows text
    [SerializeField] private TextMeshProUGUI keyPickedText;     // TMP text element
    [SerializeField] private float displayTime = 3f; // how long the message shows

    private Coroutine hideRoutine;
    public static ItemPickedKey_Water Instance; // easy global access

    private void Awake()
    {
        Instance = this;
        // Hide the UI at start
        if (keyPickedUI != null)
            keyPickedUI.SetActive(false);
    }

    /// <summary>
    /// Call this when a key item is picked up.
    /// </summary>
    public void ShowPickedKey(string keyName)
    {
        if (string.IsNullOrEmpty(keyName)) return;

        if (keyPickedUI == null || keyPickedText == null)
        {
            Debug.LogWarning("ItemPickedKey_Water: UI references not set!");
            return;
        }

        // Show message
        keyPickedUI.SetActive(true);
        keyPickedText.text = $"Picked up: {keyName}";

        // Restart timer
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        keyPickedUI.SetActive(false);
    }
}