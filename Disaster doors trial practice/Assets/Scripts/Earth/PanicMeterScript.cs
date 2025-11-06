
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // need this for slider
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PanicMeterScript : MonoBehaviour
{
    public Slider panicMeterSlider;
    //public TextMeshProUGUI healthBarValueText; // the text that says 100/100
    public int maxHealth; // maximum health
    public float currHealth; // current health
                             // Start is called before the first frame updat

    public GameObject panickEffectUI;

    private bool heartbeatPlaying = false; // track loop state

    [Header("Post Processing/ Camera blur effect")]
    public Volume volume;
    private DepthOfField dof;

    public Image fill;
    void Start()
    {
        currHealth = 0; // set the current health to max health
                        // Update is called once per frame

        panickEffectUI.SetActive(false);

        if (volume.profile.TryGet(out dof))
        {
            Debug.Log("Depth of Field found!");
        }
        else
        {
            Debug.LogWarning("No Depth of Field override found in this Volume profile!");
        }

    }

    void Update()
    {
        // set the health bar text
        //  healthBarValueText.text = currHealth.ToString() + "/" + maxHealth.ToString();
        //set the slider values
        panicMeterSlider.value = currHealth;
        panicMeterSlider.maxValue = maxHealth;

        // Example: change fill color if above 50

        if (currHealth >= 80f)
        {
            fill.color = new Color32(241, 75, 65, 255); //#f14b41


            //Panic
            panickEffectUI.SetActive(true);
            EnableBlur(true);
            if (!heartbeatPlaying)
            {
              AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[17]); // heartbeat loop
                heartbeatPlaying = true;
            }
            Debug.Log("color changed to red");
        }


        else if (currHealth >= 70f)
        {
            fill.color = new Color32(241, 75, 65, 255); //#f14b41


            //Panic
            panickEffectUI.SetActive(true);
            EnableBlur(false); //no blur
            if (!heartbeatPlaying)
            {
          AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[17]); // heartbeat loop
                heartbeatPlaying = true;
            }
            Debug.Log("color changed to red");
        }




        else if (currHealth >= 50f)
        {
            fill.color = new Color32(255, 119, 196, 255);//#FF77C4
            Debug.Log("color changed to pink");
            
                 // Calm
            panickEffectUI.SetActive(false);
            EnableBlur(false);

            if (heartbeatPlaying)
            {
                  AudioManager.Instance.StopLoop();
                heartbeatPlaying = false;
            }
        }

        else
        {
            fill.color = new Color32(242, 175, 255, 255); // #F2AFFF

            // Calm
            panickEffectUI.SetActive(false);
            EnableBlur(false);

            if (heartbeatPlaying)
            {
                  AudioManager.Instance.StopLoop();
                heartbeatPlaying = false;
            }
        }

    }
        

        public void EnableBlur(bool enable)
{
    if (dof != null)
    {
        dof.active = enable;
    }
}
}





//     void Update()
// {
//     if (panicMeterScript.currHealth >= 75)
//     {
//         // PANIC MAX — blur and heartbeat

//     }
//     else if (panicMeterScript.currHealth >= 60)
//     {
//         // Mild panic — only UI
//         panickEffectUI.SetActive(true);
//         EnableBlur(false);

//         if (heartbeatPlaying)
//         {
//             aud.StopLoop();
//             heartbeatPlaying = false;

//         }
//     }

// }
    




