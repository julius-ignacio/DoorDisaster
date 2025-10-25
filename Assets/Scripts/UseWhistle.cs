using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // if you're using TextMeshPro

public class UseWhistle : MonoBehaviour
{
    [Header("Whistle Settings")]
    public float outlineDuration = 5f;         // How long outlines stay visible
    public KeyCode whistleKey = KeyCode.F;     // Which key activates the whistle

    [Header("Targets")]
    public GameObject[] outlinedObjects;       // Objects to highlight

    private bool isUsingWhistle = false;
    public RectTransform ButtonSkill;
    private Vector2 originalPosition;

    [Header("Cooldown Settings")]
    public float cooldown = 20f;  

    [Header("UI Elements")]
    public TMP_Text cooldownText; // You can also use "public Text cooldownText;" if using the old UI Text

    void Start()
    {
        originalPosition = ButtonSkill.anchoredPosition; // Store button's starting position

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false); // Hide at start
    }

    public void Whistle()
    {
        if (!isUsingWhistle)
        {
            StartCoroutine(ActivateWhistleSkill());
        }
    }

    private IEnumerator ActivateWhistleSkill()
    {
        isUsingWhistle = true;

        // Move button off-screen instantly
        ButtonSkill.anchoredPosition = new Vector2(2000f, ButtonSkill.anchoredPosition.y);
                // Start cooldown countdown text
        if (cooldownText != null)
            StartCoroutine(ShowCooldownTimer());

        // Play sound effect
        AudioManager.Instance.PlaySFX(21);

        // Enable silhouette outlines
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.OutlineMode = Outline.Mode.SilhouetteOnly;
                outline.OutlineWidth = 8f;
            }
        }

        // Wait for outlines to stay active
        yield return new WaitForSeconds(outlineDuration);

        // Disable or revert outlines
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
        }



        // Wait for cooldown
        yield return new WaitForSeconds(cooldown);

        // Move button back to its original position
        ButtonSkill.anchoredPosition = originalPosition;

        // Allow the skill to be used again
        isUsingWhistle = false;
    }

    private IEnumerator ShowCooldownTimer()
    {
        cooldownText.gameObject.SetActive(true);

        float remaining = cooldown;

        while (remaining > 0)
        {
            cooldownText.text = Mathf.Ceil(remaining).ToString(); // Round up for cleaner numbers
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        cooldownText.gameObject.SetActive(false);
    }
}
