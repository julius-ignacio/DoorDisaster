using UnityEngine;

public class ScarySFXTrigger : MonoBehaviour
{
    public AudioManager aud;
    public int scarySFXindex;
    public GameObject PlaneTriggerDestroy;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aud.audClip.volume = 0.5f;
            aud.PlaySFX(scarySFXindex);
            // PlaneTriggerDestroy.SetActive(false);
        }
    }
    
      void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            aud.audClip.volume = 1f;
        } 
    }
}
