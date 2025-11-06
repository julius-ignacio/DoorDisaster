using UnityEngine;

public class ScarySFXTrigger : MonoBehaviour
{
    public int scarySFXindex;
    public GameObject PlaneTriggerDestroy;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
              AudioManager.Instance.audClip.volume = 0.5f;
              AudioManager.Instance.PlaySFX(scarySFXindex);
            // PlaneTriggerDestroy.SetActive(false);
        }
    }
    
      void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
              AudioManager.Instance.audClip.volume = 1f;
        } 
    }
}
