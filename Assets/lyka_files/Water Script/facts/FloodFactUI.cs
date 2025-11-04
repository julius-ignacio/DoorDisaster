using UnityEngine;
using TMPro;
using System.Collections;

public class FloodFactUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text subtitleText; // The TMP text that shows the fact

    [Header("Display Settings")]
    public float fadeDuration = 0.5f;    // Fade in/out speed
    public float displayDuration = 3f;   // How long text stays visible

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (subtitleText == null)
            subtitleText = GetComponentInChildren<TMP_Text>();

        // Add CanvasGroup if missing (controls fade)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f; // Start invisible
    }

    /// <summary>
    /// Set the fact text and display it like a subtitle
    /// </summary>
    public void SetFact(FloodFact fact)
    {
        if (fact == null || subtitleText == null)
            return;

        subtitleText.text = $"{fact.factTitle}: {fact.factDescription}";
        StopAllCoroutines();
        StartCoroutine(ShowAndHide());
    }

    private IEnumerator ShowAndHide()
    {
        // Fade in
        yield return Fade(0f, 1f, fadeDuration);

        // Wait while visible
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return Fade(1f, 0f, fadeDuration);

        Destroy(gameObject);
    }

    private IEnumerator Fade(float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }
}
