using UnityEngine;

public class GameMenu_switchers : MonoBehaviour
{
    public GameObject mainMenu;
    public RectTransform informationMenu; // ✅ use RectTransform instead of GameObject
    public GameObject disasterUpdates;
    public GameObject hotlines;
    public Eq_info_loader loader;

    private Vector2 hiddenPosition = new Vector2(0, 2224);
    private Vector2 visiblePosition = new Vector2(0, 0);

    void Start()
    {
        // Move InformationMenu off-screen at start
        informationMenu.anchoredPosition = hiddenPosition;

        hotlines.SetActive(false);
        disasterUpdates.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenInformationMenu()
    {
        informationMenu.anchoredPosition = visiblePosition;

        disasterUpdates.SetActive(true);
        mainMenu.SetActive(false);

        // ✅ Trigger data loading when user opens the menu
        if (loader != null)
            StartCoroutine(loader.LoadEarthquakeData());
    }

    public void CloseInformationMenu()
    {
        informationMenu.anchoredPosition = hiddenPosition;

        disasterUpdates.SetActive(false);
        hotlines.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OpenDisasterUpdates()
    {
        disasterUpdates.SetActive(true);
        hotlines.SetActive(false);

        // Optionally reload data each time
        if (loader != null)
            StartCoroutine(loader.LoadEarthquakeData());
    }

    public void OpenHotlines()
    {
        disasterUpdates.SetActive(false);
        hotlines.SetActive(true);
    }
}
