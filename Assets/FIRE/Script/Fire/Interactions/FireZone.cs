using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FireZone : MonoBehaviour
{
    public int fireDamage = 1;
    public float damageInterval = 3f;
    private float nextDamageTime;
    public float flashDuration = 0.4f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.5f); // semi-transparent red
               private Color originalColor;


    public Image takeDamageImage;              // UI Image overlay (set in Inspector)


    void Start()
    {
        takeDamageImage.enabled = false; // start hidden
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Movements2 player = other.GetComponent<Movements2>();
            if (player != null && Time.time >= nextDamageTime)
            {
                player.TakeDamage(fireDamage);
                     takeDamageImage.enabled = true; // start hidden
                AudioManager.Instance.PlaySFX(16); // Play fire damage sound
                     StartCoroutine(FlashDamage());

                nextDamageTime = Time.time + damageInterval;
                Debug.Log("Player damaged! HP now: " + player.currentHealth);
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
