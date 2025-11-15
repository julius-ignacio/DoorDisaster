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
    public OxygenMeterScript oxygenMeterScript;
    public ConsistentQuake consistentQuake;
    public RisingWater risingWater;


    [Header("FIRE")]
    public PlayerOxygen playerOxygen;
    public Movements2 health;


    private bool isVisible = false; // tracks if tips are currently shown

    [SerializeField] private List<GameObject> pages;
    private int currentPage = 0;

    void Start()
    {
        SetActiveSafe(Body2, false);
        SetActiveSafe(Body1, true);
        SetActiveSafe(Body3, false);
        SetActiveSafe(HUD, false);
        SetActiveSafe(PauseBtn, false);
        SetActiveSafe(PrevBtn, false);
        SetActiveSafe(startPanelUI, false);
        SetActiveSafe(whistleSkill, false);

        if (panicMeterScript != null) panicMeterScript.enabled = false;
        if (oxygenMeterScript != null) oxygenMeterScript.enabled = false;
        if (consistentQuake != null) consistentQuake.enabled = false;
        if (risingWater != null) risingWater.enabled = false;



        if (health != null) health.enabled = false;
        if (playerOxygen != null) playerOxygen.enabled = false;

        

        if (PlayerMovements != null)
        {
            PlayerMovements.speed = 0f;
            PlayerMovements.jumpHeight = 0f;
        }
    }

    public void NextPage()
    {
        if (pages != null && pages.Count > 0)
        {
            if (currentPage < pages.Count - 1)
            {
                if (currentPage >= 0 && currentPage < pages.Count)
                    SetActiveSafe(pages[currentPage], false);

                currentPage++;

                if (currentPage >= 0 && currentPage < pages.Count)
                    SetActiveSafe(pages[currentPage], true);
            }
            else
            {
                SetActiveSafe(IntroPanel, false);
                SetActiveSafe(startPanelUI, true);
            }
        }

        UpdateButtons();
    }

    public void PrevPage()
    {
        if (pages != null && pages.Count > 0)
        {
            if (currentPage > 0)
            {
                if (currentPage >= 0 && currentPage < pages.Count)
                    SetActiveSafe(pages[currentPage], false);

                currentPage--;

                if (currentPage >= 0 && currentPage < pages.Count)
                    SetActiveSafe(pages[currentPage], true);
            }
        }

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        SetActiveSafe(PrevBtn, currentPage > 0);
        // NextBtn could be toggled here if needed:
        // SetActiveSafe(NextBtn, pages != null && currentPage < pages.Count - 1);
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(33);
        }

        SetActiveSafe(startPanelUI, false);

        if (panicMeterScript != null) panicMeterScript.enabled = true;
        if (consistentQuake != null) consistentQuake.enabled = true;
        if (oxygenMeterScript != null) oxygenMeterScript.enabled = true;
        if (risingWater != null) risingWater.enabled = true;


        if (health != null) health.enabled = true;
        if (playerOxygen != null) playerOxygen.enabled = true;

        SetActiveSafe(HUD, true);
        SetActiveSafe(PauseBtn, true);

        if (PlayerMovements != null)
        {
            var Data = DataManager.Instance.currentTrial;
            switch (Data)
            {
                case 1:
                    PlayerMovements.speed = 5f;
                    PlayerMovements.jumpHeight = 1f;
                    break;
                case 2:
                    PlayerMovements.speed = 4f;
                    PlayerMovements.jumpHeight = 1.5f;
                    break;
                default:
                    PlayerMovements.speed = 5f;
                    break;
            }
        }
    }

    private static void SetActiveSafe(GameObject go, bool state)
    {
        if (go != null) go.SetActive(state);
    }
}