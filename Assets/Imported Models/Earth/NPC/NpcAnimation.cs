using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    public Animator npcAnimator;
    public float disappearDelay = 5f;
    public GameObject npcModel;
    public AudioManager aud;
    public HeartSys heart;
    public GameObject GreenFlashEffect, blueFlashEffect;
    public PanicMeterScript panicMeter;

    void Start()
    {
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();

        if (npcModel == null)
            npcModel = this.gameObject;


        GreenFlashEffect.SetActive(false);
        blueFlashEffect.SetActive(false);

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


        if (npcModel.GetComponent<Animator>())
        {
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
        }
        else
        {
            if (score != 0 && currentNpcId >= 6 && currentNpcId <= 8)
            {
                heart.Heal(1);
                aud.PlaySFX(19);
                GreenFlashEffect.SetActive(true);
                StartCoroutine(FlashFade(GreenFlashEffect.GetComponent<CanvasGroup>(), 1));
            }

            else if (score != 0 && (currentNpcId >= 9 || currentNpcId <= 11))
            {
                panicMeter.currHealth -= 20;
                aud.PlaySFX(18);
                blueFlashEffect.SetActive(true);
                StartCoroutine(FlashFade(blueFlashEffect.GetComponent<CanvasGroup>(), 1f));
            }

        }




        StartCoroutine(DisappearAfterDelay(disappearDelay));
    }



    private System.Collections.IEnumerator DisappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (npcModel != null)
            npcModel.SetActive(false);
    }


    private IEnumerator FlashFade(CanvasGroup flashGroup, float duration)
{
    flashGroup.gameObject.SetActive(true);
    flashGroup.alpha = 1f;
    yield return new WaitForSeconds(duration);
    flashGroup.alpha = 0f;
    flashGroup.gameObject.SetActive(false);
}

}
