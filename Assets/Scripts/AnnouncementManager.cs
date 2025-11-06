using System.Collections;
using UnityEngine;

public class AnnouncementManager : MonoBehaviour
{
    public GameObject subs, isFireDone, isWaterDone, isEarthDone;
    public Achievement_Almanac_Manager_Profile achievementsManager;

    void Start()
    {
        if (subs == null || isFireDone == null || isWaterDone == null || isEarthDone == null)
        {
            Debug.LogError("AnnouncementManager: One or more UI references are not assigned in the Inspector.", this);
            return;
        }

        subs.SetActive(true);
        isFireDone.SetActive(false);
        isWaterDone.SetActive(false);
        isEarthDone.SetActive(false);
    }

    void Update()
    {
        if (DataManager.Instance == null || DataManager.Instance.playerData == null) return;

        var pd = DataManager.Instance.playerData;

        if (pd.isFireFinished) { subs.SetActive(false); isFireDone.SetActive(true); }
        if (pd.isWaterFinished) { isFireDone.SetActive(false); isWaterDone.SetActive(true); }
        if (pd.isEarthFinished)
        {
            isWaterDone.SetActive(false);
            isEarthDone.SetActive(true);
            StartCoroutine(End_isEarthDoneAnnouncement());
        }
    }

    private IEnumerator End_isEarthDoneAnnouncement()
    {
        yield return new WaitForSeconds(3f);
        if (isEarthDone != null) isEarthDone.SetActive(false);
    }
}