using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class GameStart : MonoBehaviour  //, IPointerClickHandler
{
    public GameObject Header1, Header2, Body1, Body2, Bg, IntroPanel, HeaderLine, FooterLine, Lines, QuakeIcon, SlowIcon, NextBtn, PrevBtn, PanicMeter;
    public int currentBatchIndex = 0;
    public GameObject[] ObjectsToBeActive;
    public GameObject[] ObjectsToBeInactive;

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

        foreach (GameObject obj in ObjectsToBeActive)
    {
        if (obj != null)
            obj.SetActive(false);
    }
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

            if (Body2)
            {
                QuakeIcon.SetActive(true);
                SlowIcon.SetActive(true);
            }
            else { QuakeIcon.SetActive(false); SlowIcon.SetActive(false); }











            Bg.SetActive(true); // show background if you want
            PrevBtn.SetActive(true);
        }
        else
        {
            IntroPanel.SetActive(false); // hide intro panel


            // add here setting joysticks to enable!!!!
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
