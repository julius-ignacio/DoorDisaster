using UnityEngine;
using UnityEngine.UI;

public class FloodGameStart : MonoBehaviour
{
    [Header("UI References")]
    public GameObject Header1, Header2;
    public GameObject Body1, Body2;
    public GameObject Bg;
    public GameObject IntroPanel;
    public GameObject HeaderLine, FooterLine, Lines;
    public GameObject OxygenIcon, HeartIcon;
    public GameObject NextBtn, PrevBtn;
    public GameObject HUD, PauseBtn;

    [Header("Player & Narration")]
    public PlayerController_Water PlayerController;  // ✅ Use the correct player script
    public GameObject GameStartNarration;      // Player talk/narration

    private bool isVisible = false;

    void Start()
    {
        // ---------------- Initial UI Setup ----------------
        Header2.SetActive(false);
        Header1.SetActive(true);
        Body2.SetActive(false);
        Body1.SetActive(true);
        OxygenIcon.SetActive(false);
        HeartIcon.SetActive(false);
        Lines.SetActive(false);
        HeaderLine.SetActive(true);
        FooterLine.SetActive(true);
        HUD.SetActive(false);
        PauseBtn.SetActive(false);
        GameStartNarration.SetActive(false);

        // Disable player movement during intro
        if (PlayerController != null)
        {
            PlayerController.speed = 0f;
            PlayerController.jumpHeight = 0f;
        }
    }

    public void Next()
    {
        if (!isVisible)
        {
            // ---------------- Page 2 (Info) ----------------
            Header2.SetActive(true);
            Header1.SetActive(false);
            HeaderLine.SetActive(false);
            FooterLine.SetActive(false);
            Body2.SetActive(true);
            Body1.SetActive(false);
            Lines.SetActive(true);
            OxygenIcon.SetActive(true);
            HeartIcon.SetActive(true);
            Bg.SetActive(true);
            PrevBtn.SetActive(true);
        }
        else
        {
            // ---------------- Start Game ----------------
            IntroPanel.SetActive(false);
            HUD.SetActive(true);
            PauseBtn.SetActive(true);
            GameStartNarration.SetActive(true);

            // Enable player movement
            if (PlayerController != null)
            {
                PlayerController.speed = 20f;      // your normal speed
                PlayerController.jumpHeight = 7f;  // your normal jump
            }
        }

        isVisible = !isVisible;
    }

    public void Prev()
    {
        if (isVisible)
        {
            // ---------------- Back to Page 1 ----------------
            Header1.SetActive(true);
            Header2.SetActive(false);
            HeaderLine.SetActive(true);
            FooterLine.SetActive(true);
            Body1.SetActive(true);
            Body2.SetActive(false);
            OxygenIcon.SetActive(false);
            HeartIcon.SetActive(false);
            Bg.SetActive(true);
            Lines.SetActive(false);
            PrevBtn.SetActive(false);
        }
        else
        {
            IntroPanel.SetActive(true);
        }

        isVisible = !isVisible;
    }
}
