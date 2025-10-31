using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public GameObject AllDoorsCompleted, firedone, waterdone, earthdone, AlmanacUI;
    void Start()
    {
        AllDoorsCompleted.SetActive(false);
        AlmanacUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkIFAchievementsIsDone();
    }


public void checkIFAchievementsIsDone()
    {
         var playerData = DataManager.Instance.playerData;

        if (playerData != null &&
            playerData.isEarthFinished &&
            playerData.isWaterFinished &&
            playerData.isFireFinished)
        { AllDoorsCompleted.SetActive(true); }
        else { AllDoorsCompleted.SetActive(false); }


        if (playerData != null &&
            playerData.isFireFinished)
        { firedone.SetActive(true); }
        else { firedone.SetActive(false); }



        if (playerData != null &&
          playerData.isWaterFinished)
        { waterdone.SetActive(true); }
        else { waterdone.SetActive(false); }
        

          if (playerData != null &&
            playerData.isEarthFinished)
        { earthdone.SetActive(true); }
        else { earthdone.SetActive(false); }
    }



    public void CloseAlmanac()
    {
        AlmanacUI.SetActive(false);
    }
    
        public void OpenAlmanac()
    {
        AlmanacUI.SetActive(true);
    }
}
