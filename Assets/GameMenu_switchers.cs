using UnityEngine;

public class GameMenu_switchers : MonoBehaviour
{
    public GameObject mainMenu, informationMenu;
    public GameObject disasterUpdates, hotlines;

    void Start()
    {
        informationMenu.SetActive(false);
        hotlines.SetActive(false);
        disasterUpdates.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenInformationMenu()
    {
        informationMenu.SetActive(true);
        disasterUpdates.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OpenDisasterUpdates()
    {
        disasterUpdates.SetActive(true);
        hotlines.SetActive(false);
    }

    public void OpenHotlines()
    {
        disasterUpdates.SetActive(false);
        hotlines.SetActive(true);
    }
}
