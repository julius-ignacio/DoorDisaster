
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
                           // Start is called before the first frame update
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
    }
}