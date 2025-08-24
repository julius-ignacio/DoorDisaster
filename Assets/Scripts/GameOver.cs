using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject Joystick, Jumpbtn, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm;
    public PanicMeterScript panicMeterScript;
    public Movements movementscript;



    public void playerGameOver()
    {
        Joystick.SetActive(false);
        Jumpbtn.SetActive(false);
        GameOverUI.SetActive(true);
        PanicMeterUI.SetActive(false);
        CoverBtn.SetActive(false);
        uncoverBtm.SetActive(false);

        movementscript.enabled = false;
    }


    void Update()
    {
        if (panicMeterScript.currHealth >= 100)
        {
            playerGameOver();
        }
    }
}
