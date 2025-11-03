using System.Collections;
using UnityEngine;

public class AnnouncementManager : MonoBehaviour
{
    public GameObject subs, isFireDone, isWaterDone, isEarthDone;
    public Achievement_Almanac_Manager achievementsManager;
    // Update is called once per frame

    void Start()
    {
        subs.SetActive(true);
        isFireDone.SetActive(false);
        isWaterDone.SetActive(false);
        isEarthDone.SetActive(false);
    }
    void Update()
    {

        if (DataManager.Instance.playerData.isFireFinished == true) { subs.SetActive(false); }
        if (DataManager.Instance.playerData.isFireFinished == true) { isFireDone.SetActive(true); }


        if (DataManager.Instance.playerData.isWaterFinished == true) { isFireDone.SetActive(false); }
        if (DataManager.Instance.playerData.isWaterFinished == true) { isWaterDone.SetActive(true); }



        if (DataManager.Instance.playerData.isEarthFinished == true) { isWaterDone.SetActive(false); }
        if (DataManager.Instance.playerData.isEarthFinished == true) {
            isEarthDone.SetActive(true);

            // Start the coroutine to disable the Earth announcement after 3 seconds
            StartCoroutine(End_isEarthDoneAnnouncement());
     }

    }
    


private IEnumerator End_isEarthDoneAnnouncement()
{
    yield return new WaitForSeconds(3f);
        isEarthDone.SetActive(false);
}
}
