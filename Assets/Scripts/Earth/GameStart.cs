using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class GameStart : MonoBehaviour  //, IPointerClickHandler
{
    public GameObject Body1, Body2, Body3, Bg, IntroPanel,
    NextBtn, PrevBtn, HUD, PauseBtn, startPanelUI, whistleSkill;


    // public GameObject GamestartNarration; //Game start player narration /talking
    public int currentBatchIndex = 0;
    public Movements PlayerMovements; //disables the player from moving until the intro is done/intro panel is closed

    public PanicMeterScript panicMeterScript;
    public ConsistentQuake consistentQuake;


    private bool isVisible = false; // tracks if tips are currently shown

    [SerializeField] private List<GameObject> pages;
    private int currentPage = 0;


    void Start()
    {

        Body2.SetActive(false);
        Body1.SetActive(true);
        Body3.SetActive(false);
        HUD.SetActive(false);
        PauseBtn.SetActive(false);
        PrevBtn.SetActive(false);
        startPanelUI.SetActive(false);
        panicMeterScript.enabled = false;
        consistentQuake.enabled = false;
        whistleSkill.SetActive(false);


        //    GamestartNarration.SetActive(false);

        PlayerMovements.speed = 0f;
        PlayerMovements.jumpHeight = 0f;
    }


    public void NextPage()
    {
        if (currentPage < pages.Count - 1)
        {
            pages[currentPage].SetActive(false);
            currentPage++;
            pages[currentPage].SetActive(true);
        }
        else
        {
            // Last page reached — show start panel
            IntroPanel.SetActive(false);
            startPanelUI.SetActive(true);
        }

        UpdateButtons();
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            pages[currentPage].SetActive(false);
            currentPage--;
            pages[currentPage].SetActive(true);
        }

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        PrevBtn.SetActive(currentPage > 0);
        // NextBtn.SetActive(currentPage < pages.Count - 1);
    }


    public void StartGame()
    {

        AudioManager.Instance.PlaySFX(33);

        // hide the black start panel
        startPanelUI.SetActive(false);
        panicMeterScript.enabled = true;
        consistentQuake.enabled = true;
        // enable game HUD
        HUD.SetActive(true);
        PauseBtn.SetActive(true);
        //  GamestartNarration.SetActive(true);

        // enable player movement
        PlayerMovements.speed = 3f;
        PlayerMovements.jumpHeight = 1f;
    }


    // private IEnumerator StartSequence()
    // {

    //     // Wait another 1 second, then trigger the quake and panic meter
    //     yield return new WaitForSeconds(1f);



    // }
}
