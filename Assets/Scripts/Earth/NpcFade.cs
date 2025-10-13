using UnityEngine;

public class NpcFade : MonoBehaviour
{
  public float fadeDuration = 2f; // seconds
    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    private System.Collections.IEnumerator FadeOutRoutine()
    {
        float elapsed = 0f;
        Color c = originalColor;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            c.a = alpha;
            rend.material.color = c;
            yield return null;
        }

        // Optionally disable after fade
        gameObject.SetActive(false);
    }
}
