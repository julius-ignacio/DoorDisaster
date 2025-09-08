using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class GameStart : MonoBehaviour  //, IPointerClickHandler
{
    public GameObject Header1, Header2, Body1, Body2, Bg, IntroPanel, HeaderLine,
    FooterLine, Lines, QuakeIcon, SlowIcon,
    NextBtn, PrevBtn, PanicMeter, HUD, PauseBtn;


    public GameObject GamestartNarration; //Game start player narration /talking
    public int currentBatchIndex = 0;
    public Movements PlayerMovements; //disables the player from moving until the intro is done/intro panel is closed

    private bool isVisible = false; // tracks if tips are currently shown


    void Start()
    {
        Header2.SetActive(false);
        Header1.SetActive(true);
        Body2.SetActive(false);
        Body1.SetActive(true);
        QuakeIcon.SetActive(false);
        SlowIcon.SetActive(false);
        PanicMeter.SetActive(false);
        Lines.SetActive(false);
        HeaderLine.SetActive(true);
        FooterLine.SetActive(true);
        HUD.SetActive(false);
        PauseBtn.SetActive(false);

        GamestartNarration.SetActive(false);

        PlayerMovements.speed = 0f;
        PlayerMovements.jumpHeight = 0f;
    }

    public void Next()
    {
        if (!isVisible)
        {
            Header2.SetActive(true);
            Header1.SetActive(false);
            HeaderLine.SetActive(false);
            FooterLine.SetActive(false);
            PanicMeter.SetActive(true);
            Body2.SetActive(true);
            Body1.SetActive(false);
            Lines.SetActive(true);
            QuakeIcon.SetActive(true);
            SlowIcon.SetActive(true);
            

            Bg.SetActive(true); // show background if you want
            PrevBtn.SetActive(true);
        }
        else
        {
            IntroPanel.SetActive(false); // hide intro panel


            // add here setting joysticks to enable!!!!
            HUD.SetActive(true);


            //enable player movments
            PlayerMovements.speed = 3f;
            PlayerMovements.jumpHeight = 1f;

            //pausebtn active
        PauseBtn.SetActive(true);


            //player talks
            GamestartNarration.SetActive(true);
        }

        isVisible = !isVisible; // flip state
    }



    public void Prev()
    {
        if (isVisible)
        {
            // Switch back headers
            Header1.SetActive(true);
            Header2.SetActive(false);
            HeaderLine.SetActive(true);
            FooterLine.SetActive(true);

            // Switch back bodies
            Body1.SetActive(true);
            Body2.SetActive(false);

            // Hide icons
            QuakeIcon.SetActive(false);
            SlowIcon.SetActive(false);

            // Optionally hide background
            Bg.SetActive(true);

            PanicMeter.SetActive(false);
            Lines.SetActive(false);

            // Hide prev button if we're back at first batch
            PrevBtn.SetActive(false);
        }
        else
        {
            // If already at intro, you might want to reopen it or do nothing
            IntroPanel.SetActive(true);

            // disable joysticks again if needed
        }

        // Flip the state
        isVisible = !isVisible;
    }

}
