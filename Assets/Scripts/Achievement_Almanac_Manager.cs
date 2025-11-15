using UnityEngine;

public class Achievement_Almanac_Manager_Profile : MonoBehaviour
{
    public GameObject AllDoorsCompleted, waterdone, earthdone, firedone, AchievementsUI, AlmanacUI, ProfileUI;

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
            playerData.isEarthFinishedNormal &&
            playerData.isWaterFinishedNormal &&
            playerData.isFireFinishedNormal)
        {
            AchievementUnlocker(AllDoorsCompleted.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(AllDoorsCompleted.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
            playerData.isFireFinishedNormal)
        {
            AchievementUnlocker(firedone.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(firedone.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
          playerData.isWaterFinishedNormal)
        {
            AchievementUnlocker(waterdone.GetComponent<UnityEngine.UI.Image>());
        }
        else
        {
            AchievementLocker(waterdone.GetComponent<UnityEngine.UI.Image>());
        }


        if (playerData != null &&
          playerData.isEarthFinishedNormal)
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




    public void ToggleAlmanac()
    {
        AlmanacUI.SetActive(!AlmanacUI.activeSelf);
        ProfileUI.SetActive(false);
        AchievementsUI.SetActive(false);


    }

        public void ToggleProfile()
    {
        ProfileUI.SetActive(!ProfileUI.activeSelf);
        AlmanacUI.SetActive(false);
        AchievementsUI.SetActive(false);
    }

       public void ToggleAchieve()
    {
        AchievementsUI.SetActive(!AchievementsUI.activeSelf);
        AlmanacUI.SetActive(false);
        ProfileUI.SetActive(false);
    }
}
