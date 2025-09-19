using System;
using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    public Animator npcAnimator;
    public float disappearDelay = 5f;
    public GameObject npcModel;
    public AudioManager aud;

    void Start()
    {
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();

        if (npcModel == null)
            npcModel = this.gameObject;

        // if (aud == null)
       // aud = FindObjectOfType<AudioManager>();
    }

public void PlayAndDisappear(int currentNpcId)
{
    int score = 0;

    if (DataManager.Instance.npcScores.TryGetValue(currentNpcId, out score))
    {
        Debug.Log($"NPC {currentNpcId} score found: {score}");
    }
    else
    {
        Debug.LogWarning($"No score found for NPC {currentNpcId}, defaulting to 0");
    }

    if (score == 3)
    {
        npcAnimator.SetTrigger("Victory");
        aud.PlaySFX(0);
    }
    else if (score == 0)
    {
        npcAnimator.SetTrigger("Death");
        aud.PlaySFX(2);
    }
    else if (score == 1 || score == 2)
    {
        npcAnimator.SetTrigger("Clap");
        aud.PlaySFX(3);
    }

    StartCoroutine(DisappearAfterDelay(disappearDelay));
}



    private System.Collections.IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (npcModel != null)
            npcModel.SetActive(false);
    }
}
