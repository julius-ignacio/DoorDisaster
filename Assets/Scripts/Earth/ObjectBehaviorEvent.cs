using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ObjectBehaviorEvent : MonoBehaviour
{
    public Animator npcAnimator;
    public float disappearDelay = 5f;
    public GameObject npcModel;
    public HeartSys heart;
    public UseWhistle useWhistle;
    public GameObject whistleCD_UI;
    public GameObject GreenFlashEffect, BlueFlashEffect, YellowFlashEffect;
    public PanicMeterScript panicMeter;
    public GameNotifier gameNotifier;
    public Objectives objectives;
    public InventoryManager inventory;


    [Header("Npc icons")]
    public NpcsSaved npcsaved;

    void Start()
    {
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();

        if (npcModel == null)
            npcModel = this.gameObject;

        whistleCD_UI.SetActive(false);


        GreenFlashEffect.SetActive(false);
        BlueFlashEffect.SetActive(false);
        YellowFlashEffect.SetActive(false);

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
            if (score != 0 && currentNpcId >= 6 && currentNpcId <= 9) //medkit
            {
                AudioManager.Instance.PlaySFX(8); //points
                gameNotifier.EarnedPoints(score);
                inventory.medkit++;
            }

            else if (score != 0 && currentNpcId >= 10 && currentNpcId <= 13) //waterbottle
            {
                AudioManager.Instance.PlaySFX(8); //points
                gameNotifier.EarnedPoints(score);
                inventory.water++;
            }

            else if (score != 0 && currentNpcId == 14)//whistle
            {
                AudioManager.Instance.PlaySFX(8); //points
                Debug.Log($"[DEBUG] heart={heart}, useWhistle={useWhistle}, gameNotifier={gameNotifier}, objectives={objectives}");

                useWhistle.ButtonSkill.gameObject.SetActive(true);
                whistleCD_UI.SetActive(true);


                gameNotifier.ObtainedItem(score, "Whistle");
            }

            else if (score != 0 && currentNpcId == 15) //helmet
            {
                AudioManager.Instance.PlaySFX(8); //points
                heart.HelmetUsed();
                gameNotifier.ObtainedItem(score, "Safety helmet");

                YellowFlashEffect.SetActive(true);
                StartCoroutine(FlashFade(YellowFlashEffect.GetComponent<CanvasGroup>(), 1f));
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




    // public void DrinkWater()
    // {
    //     if (panic.currHealth != 0 && panic.currHealth >= 20)
    //     {
    //         water--;
    //         waterCounter.text = water.ToString();

    //         inventoryUI.SetActive(false);
    //         panic.currHealth -= 20;
    //         AudioManager.Instance.PlaySFX(18);

    //         BlueFlashEffect.SetActive(true);
    //         StartCoroutine(FlashFade(BlueFlashEffect.GetComponent<CanvasGroup>(), 1f));
    //     }

    //     else if(panic.currHealth < 20)
    //     {
    //         water--;
    //         waterCounter.text = water.ToString();

    //         inventoryUI.SetActive(false);
    //         panic.currHealth = 0;
    //         AudioManager.Instance.PlaySFX(18);

    //         BlueFlashEffect.SetActive(true);
    //         StartCoroutine(FlashFade(BlueFlashEffect.GetComponent<CanvasGroup>(), 1f));
    //     }
    // }



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
