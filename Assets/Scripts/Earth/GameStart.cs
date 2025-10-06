using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class GameStart : MonoBehaviour  //, IPointerClickHandler
{
    public GameObject Header1, Header2, Body1, Body2, Bg, IntroPanel, HeaderLine,
    FooterLine, Lines, QuakeIcon, SlowIcon,
    NextBtn, PrevBtn, PanicMeter, HUD, PauseBtn, startPanelUI;


    public GameObject GamestartNarration; //Game start player narration /talking
    public int currentBatchIndex = 0;
    public Movements PlayerMovements; //disables the player from moving until the intro is done/intro panel is closed

    public PanicMeterScript panicMeterScript;
    public ConsistentQuake consistentQuake;

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
        startPanelUI.SetActive(false);
        panicMeterScript.enabled = false;
        consistentQuake.enabled = false;


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
            // hide intro
            IntroPanel.SetActive(false);

            // show tap-to-start black screen
            startPanelUI.SetActive(true);
        }
        isVisible = !isVisible; // flip state
    }


    public void StartGame()
    {

        // hide the black start panel
        startPanelUI.SetActive(false);
        panicMeterScript.enabled = true;
        consistentQuake.enabled = true;
        // enable game HUD
        HUD.SetActive(true);
        PauseBtn.SetActive(true);
        GamestartNarration.SetActive(true);

        // enable player movement
        PlayerMovements.speed = 3f;
        PlayerMovements.jumpHeight = 1f;
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
