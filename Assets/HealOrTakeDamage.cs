using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealOrTakeDamage : MonoBehaviour
{
    public HeartSys heartSys;                  // Reference to HeartSys (drag HeartPanel here)
    public ConsistentQuake consistentQuake;    // Reference to ConsistentQuake (drag the quake manager here)
    public Image takeDamageImage;              // UI Image overlay (set in Inspector)

    public float flashDuration = 0.3f;         // how long the flash stays
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // semi-transparent red


    public AudioSource hurtSound;

    private Color originalColor;

    private void Start()
    {
        if (takeDamageImage != null)
        {
            originalColor = takeDamageImage.color;
            takeDamageImage.enabled = false; // start hidden
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (consistentQuake != null && consistentQuake.IsQuakeActive)
            {
                Debug.Log("Player hit by locker during quake! Taking damage...");
                heartSys.TakeDamage(1);
                hurtSound?.Play();

                if (takeDamageImage != null)
                    StartCoroutine(FlashDamage());
            }
        }
    }

    private IEnumerator FlashDamage()
    {
        takeDamageImage.enabled = true;
        takeDamageImage.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        takeDamageImage.enabled = false;
        takeDamageImage.color = originalColor;
    }
}
