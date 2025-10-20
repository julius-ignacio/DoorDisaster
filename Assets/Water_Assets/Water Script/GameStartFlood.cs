using UnityEngine;
using System.Collections.Generic;

public class GameStartFlood : MonoBehaviour
{
    [Header("Scene Objects")]
    public GameObject Body1, Body2, Bg, IntroPanel;
    public GameObject NextBtn, PrevBtn, HUD, PauseBtn;
    public GameObject startPanelUI;

    [Header("Narration")]
    public GameObject GameStartNarration;

    [Header("Player Settings")]
    public PlayerController_Water playerController;

    [Header("Page System")]
    [SerializeField] private List<GameObject> pages;
    private int currentPage = 0;

    private bool hasStartedGame = false;

    void Start()
    {
        // Prevent re-showing intro if already started before
        if (PlayerPrefs.GetInt("HasStartedGame", 0) == 1)
        {
            IntroPanel.SetActive(false);
            startPanelUI.SetActive(false);
            HUD.SetActive(true);
            PauseBtn.SetActive(true);
            GameStartNarration.SetActive(true);

            if (playerController != null)
            {
                playerController.speed = 3f;
                playerController.jumpHeight = 7f;
            }
            return;
        }

        // Initial setup for first-time players
        Body1.SetActive(true);
        Body2.SetActive(false);
        HUD.SetActive(false);
        PauseBtn.SetActive(false);
        PrevBtn.SetActive(false);
        startPanelUI.SetActive(false);
        GameStartNarration.SetActive(false);

        if (playerController != null)
        {
            playerController.speed = 0f;
            playerController.jumpHeight = 0f;
        }

        // Initialize the pages
        if (pages.Count > 0)
        {
            for (int i = 0; i < pages.Count; i++)
                pages[i].SetActive(i == 0);
        }

        UpdateButtons();
    }

    // ---------------- PAGE NAVIGATION ----------------
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
            // End of intro — show start panel
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
    }

    // ---------------- START GAME ----------------
    public void StartGame()
    {
        startPanelUI.SetActive(false);
        HUD.SetActive(true);
        PauseBtn.SetActive(true);
        GameStartNarration.SetActive(true);

        if (playerController != null)
        {
            playerController.speed = 3f;
            playerController.jumpHeight = 7f;
        }

        hasStartedGame = true;
        PlayerPrefs.SetInt("HasStartedGame", 1);
    }

    public void ResetIntro() // optional if you want to restart intro manually
    {
        PlayerPrefs.SetInt("HasStartedGame", 0);
    }
}
