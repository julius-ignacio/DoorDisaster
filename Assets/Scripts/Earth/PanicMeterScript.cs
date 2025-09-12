
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // need this for slider
using TMPro;
public class PanicMeterScript : MonoBehaviour
{
    public Slider panicMeterSlider;
    //public TextMeshProUGUI healthBarValueText; // the text that says 100/100
    public int maxHealth; // maximum health
    public float currHealth; // current health
                             // Start is called before the first frame updat

    public Image fill;
    void Start()
    {
        currHealth = 0; // set the current health to max health
                        // Update is called once per frame
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
            Debug.Log("color changed to red");
        }
        else if (currHealth >= 50f)
        {
            fill.color = new Color32(255, 119, 196, 255);//#FF77C4
            Debug.Log("color changed to pink");
        }

        else
        {
            fill.color = new Color32(242, 175, 255, 255); // #F2AFFF

        }
    }

}
