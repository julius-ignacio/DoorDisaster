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
        if (DataManager.Instance.isEartFinished && DataManager.Instance.isWaterFinished && DataManager.Instance.isFireFinished)
        {
            AllDoorsCompleted.SetActive(true);
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
