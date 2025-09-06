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
    public AudioSource lockerObjectsFallSound;

    private Color originalColor;
    private bool hasDealtDamage = false; // ✅ track if this locker has already hurt the player

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
        if (!hasDealtDamage && collision.gameObject.CompareTag("Player"))
        {
            //  if (consistentQuake != null && consistentQuake.IsQuakeActive)
            //   {
            Debug.Log("Player hit by locker during quake! Taking damage...");
            lockerObjectsFallSound?.Play();

            heartSys.TakeDamage(1);
            hurtSound?.Play();

            if (takeDamageImage != null)
                StartCoroutine(FlashDamage());

            hasDealtDamage = true; // ✅ only deal damage once
                                   // }
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
