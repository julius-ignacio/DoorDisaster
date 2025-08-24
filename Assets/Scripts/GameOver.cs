using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject Joystick, Jumpbtn, Player, GameOverUI, PanicMeterUI, CoverBtn, uncoverBtm;


public void playerGameOver()
    {
        Joystick.SetActive(false);
        Jumpbtn.SetActive(false);
        Player.SetActive(false);
        GameOverUI.SetActive(true);
        PanicMeterUI.SetActive(false);
        CoverBtn.SetActive(false);
        uncoverBtm.SetActive(false);
    }
}
