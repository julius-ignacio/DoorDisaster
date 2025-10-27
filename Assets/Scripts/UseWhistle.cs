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


    [Header("Whistle CD")]
    public GameObject cooldownUI;


    void Start()
    {
        originalPosition = ButtonSkill.anchoredPosition; // Store button's starting position

        // if (cooldownText != null)
        //     cooldownText.gameObject.SetActive(false); // Hide at start


        if (cooldownUI != null)
            cooldownUI.SetActive(false); // Hide at start
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
        cooldownUI.SetActive(true);

        // Play sound effect
        AudioManager.Instance.PlaySFX(21);

        // Enable silhouette outlines
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                outline.OutlineWidth = 3f;
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
                outline.enabled = true;
                outline.OutlineMode = Outline.Mode.OutlineAll;
                outline.OutlineWidth = 3f;


            }
        }
        
                        // Start cooldown countdown text
        if (cooldownText != null)
            StartCoroutine(ShowCooldownTimer());



        // Wait for cooldown
        yield return new WaitForSeconds(cooldown);

        // Move button back to its original position
        ButtonSkill.anchoredPosition = originalPosition;

        // Allow the skill to be used again
        isUsingWhistle = false;
    }

    private IEnumerator ShowCooldownTimer()
    {
        float remaining = cooldown;

        while (remaining > 0)
        {
            cooldownText.text = Mathf.Ceil(remaining).ToString(); // Round up for cleaner numbers
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

    }
}
