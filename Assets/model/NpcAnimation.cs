using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    public Animator npcAnimator;
    public string animationTrigger = "correctAnswer"; // The trigger name in Animator
    public float disappearDelay = 3f;        // Time before disappearing
    public GameObject npcModel;              // Reference to NPC model (child mesh/renderer)

    void Start()
    {
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();
        
        if (npcModel == null)
            npcModel = this.gameObject; // fallback: whole object
    }

    public void PlayAndDisappear(int currentNpcId)
    {
        if (npcAnimator != null)
        {


            if (DataManager.Instance.individualNpcScores[currentNpcId - 1] >= 3) // Assuming 3 is the threshold for "helped"
            {
                npcAnimator.SetTrigger(animationTrigger);
            }
            else if (DataManager.Instance.individualNpcScores[currentNpcId - 1] < 3)
            {
                npcAnimator.SetTrigger(animationTrigger);

            }
            else
            {
                npcAnimator.SetTrigger(animationTrigger);
                
            }
        }






        StartCoroutine(DisappearAfterDelay(disappearDelay));
    }

    private System.Collections.IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Option 1: just hide the model
        if (npcModel != null)
            npcModel.SetActive(false);

        // Option 2: completely remove NPC
        // Destroy(gameObject);
    }
}
