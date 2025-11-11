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
    public string itemName;


    [Header("Npc icons")]
    public NpcsSaved npcsaved;

    void Start()
    {
        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();

        if (npcModel == null)
            npcModel = this.gameObject;

        if (whistleCD_UI != null)
            whistleCD_UI.SetActive(false);

        if (GreenFlashEffect != null)
            GreenFlashEffect.SetActive(false);

        if (BlueFlashEffect != null)
            BlueFlashEffect.SetActive(false);

        if (YellowFlashEffect != null)
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


    public void PlayAndDisappear_water(int currentNpcId)
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


        if (score == 0) //medkit
        {
            AudioManager.Instance.PlaySFX(8); //points
            gameNotifier.ObtainedItem(score, itemName, 3f);
        }

        else if (score == 1 || score == 2) //waterbottle
        {
            AudioManager.Instance.PlaySFX(8); //points
                        gameNotifier.ObtainedItem(score, itemName, 3f);

        }

        else
        {
            AudioManager.Instance.PlaySFX(8); //points
                       gameNotifier.ObtainedItem(score, itemName, 3f);

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
