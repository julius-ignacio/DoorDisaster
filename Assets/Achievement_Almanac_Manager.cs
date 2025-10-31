using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Achievement_Almanac_Manager : MonoBehaviour
{
    public GameObject AllDoorsCompleted, firedone, waterdone, earthdone, AchievementsUI, AlmanacUI;

    // Update is called once per frame
    void Update()
    {
        checkIFAchievementsIsDone();
    }
    void Start()
    {
        AlmanacUI.SetActive(false);
        AchievementsUI.SetActive(false);
    }


    public void checkIFAchievementsIsDone()
    {
        var playerData = DataManager.Instance.playerData;

        if (playerData != null &&
            playerData.isEarthFinished &&
            playerData.isWaterFinished &&
            playerData.isFireFinished)
        {
            AchievementUnlocker(AllDoorsCompleted.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(AllDoorsCompleted.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
            playerData.isFireFinished)
        {
            AchievementUnlocker(firedone.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(firedone.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
          playerData.isWaterFinished)
        {
            AchievementUnlocker(waterdone.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(waterdone.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
          playerData.isEarthFinished)
        {
            AchievementUnlocker(earthdone.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(earthdone.GetComponent<UnityEngine.UI.Image>());
        }
    }


    public void AchievementLocker(UnityEngine.UI.Image imageComponent)
    {
        imageComponent.color = Color.black;
    }

    public void AchievementUnlocker(UnityEngine.UI.Image imageComponent)
    {
        imageComponent.color = Color.white;
    }




    public void CloseAlmanac()
    {
        AlmanacUI.SetActive(false);
    }

    public void OpenAlmanac()
    {
        AlmanacUI.SetActive(true);
    }

    public void CloseAchievements()
    {
        AchievementsUI.SetActive(false);
    }

    public void OpenAchievements()
    {
        AchievementsUI.SetActive(true);
    }
}
