using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public GameObject AllDoorsCompleted, AlmanacUI;
    void Start()
    {
        AllDoorsCompleted.SetActive(false);
        AlmanacUI.SetActive(false);
    }

    // Update is called once per frame
void Update()
{
    var playerData = DataManager.Instance.playerData;

    if (playerData != null &&
        playerData.isEarthFinished &&
        playerData.isWaterFinished &&
        playerData.isFireFinished)
    {
        AllDoorsCompleted.SetActive(true);
    }
        else
        {
            AllDoorsCompleted.SetActive(false);
        }
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
