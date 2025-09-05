using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject Joystick, Jumpbtn, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm, PauseUI,  heartsys;
    public PanicMeterScript panicMeterScript;
    public HeartSys hearts;
    public TMP_Text panicText, injurtyText;
    public Movements movementscript;

    public void playerGameOver()
    {
        Joystick.SetActive(false);
        Jumpbtn.SetActive(false);
        GameOverUI.SetActive(true);
        PanicMeterUI.SetActive(false);
        CoverBtn.SetActive(false);
        uncoverBtm.SetActive(false);
        PauseUI.SetActive(false);
        heartsys.SetActive(false);


        movementscript.enabled = false;

    }


    void Update()
    {
        if (panicMeterScript.currHealth >= 100)
        {
            panicText.gameObject.SetActive(true);
            injurtyText.gameObject.SetActive(false);
            playerGameOver();
        }

        if (hearts.currentHearts <= 0)
        {
            panicText.gameObject.SetActive(false);
            injurtyText.gameObject.SetActive(true);
            playerGameOver();
        }
    }
}
