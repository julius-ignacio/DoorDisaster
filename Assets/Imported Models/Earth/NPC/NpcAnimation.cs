using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class NpcAnimation : MonoBehaviour
{
    public Animator npcAnimator;
    public float disappearDelay = 5f;
    public GameObject npcModel;
    public HeartSys heart;
    public UseWhistle useWhistle;
    public GameObject GreenFlashEffect, blueFlashEffect;
    public PanicMeterScript panicMeter;
    public GameNotifier gameNotifier;
    public Objectives objectives;


    [Header("Npc icons")]
    public NpcsSaved npcsaved;

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
                AudioManager.Instance.PlaySFX(11);
                AudioManager.Instance.PlaySFX(8); //points


                DataManager.Instance.Npcs_saved++; // increment global NPCs saved count

                gameNotifier.EarnedPoints(score);
                if (npcsaved != null) { npcsaved.makeIconActive(); }

                objectives.UpdateObjectives();


            }
            else if (score == 0)
            {
                npcAnimator.SetTrigger("Death");
                AudioManager.Instance.PlaySFX(9);
                AudioManager.Instance.PlaySFX(8); //points

                gameNotifier.EarnedPoints(score);
                if (npcsaved != null) { npcsaved.makeIconActive(); }

            }
            else if (score == 1 || score == 2)
            {
                npcAnimator.SetTrigger("Clap");
                AudioManager.Instance.PlaySFX(10);
                AudioManager.Instance.PlaySFX(8); //points


                DataManager.Instance.Npcs_saved++; // increment global NPCs saved count

                gameNotifier.EarnedPoints(score);
                if (npcsaved != null) { npcsaved.makeIconActive(); }
                
                objectives.UpdateObjectives();


            }
        }
        else
        {
            if (score != 0 && currentNpcId >= 6 && currentNpcId <= 8)
            {
                heart.Heal(1);
                AudioManager.Instance.PlaySFX(19);
                AudioManager.Instance.PlaySFX(8); //points
                gameNotifier.EarnedPoints(score);


                GreenFlashEffect.SetActive(true);
                StartCoroutine(FlashFade(GreenFlashEffect.GetComponent<CanvasGroup>(), 1));
            }

            else if (score != 0 && currentNpcId >= 9 && currentNpcId <= 11)
            {
                panicMeter.currHealth -= 20;
                AudioManager.Instance.PlaySFX(18);
                AudioManager.Instance.PlaySFX(8); //points
                gameNotifier.EarnedPoints(score);


                blueFlashEffect.SetActive(true);
                StartCoroutine(FlashFade(blueFlashEffect.GetComponent<CanvasGroup>(), 1f));
            }

            else if (score != 0 && currentNpcId == 12)
            {
                AudioManager.Instance.PlaySFX(8); //points
                useWhistle.ButtonSkill.SetActive(true);
                gameNotifier.ObtainedItem(score, "Whistle");
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


// public void ReactToScore(int score)
// {
//     if (score == 3)
//     {
//         npcAnimator.SetTrigger("Victory");
//         AudioManager.Instance.PlaySFX(11);
//     }
//     else if (score == 0)
//     {
//         npcAnimator.SetTrigger("Death");
//         AudioManager.Instance.PlaySFX(9);
//     }
//     else
//     {
//         npcAnimator.SetTrigger("Clap");
//         AudioManager.Instance.PlaySFX(10);
//     }

//     // other conditions based on npcId
// }


}
