using UnityEngine;

public class TrialLockManager : MonoBehaviour
{
    [Header("Inner triggers")]
    public GameObject[] InnerTrigger;

    [Header("Glow areas")]
    public GameObject[] Glow;

    [Header("Emblems")]
    public GameObject[] Emblem;

    [Header("Material")]
    public Material LockedTrialMaterial;

    [Header("Lights")]
    public GameObject[] Light;

    void Start()
    {
        Light[0].SetActive(false);
        Light[1].SetActive(false);

        InnerTrigger[0].SetActive(false);
        InnerTrigger[1].SetActive(false);

        Glow[0].GetComponent<Renderer>().material = LockedTrialMaterial;
        Glow[1].GetComponent<Renderer>().material = LockedTrialMaterial;

        Emblem[0].GetComponent<Renderer>().material = LockedTrialMaterial;
        Emblem[1].GetComponent<Renderer>().material = LockedTrialMaterial;
    }
    void Update()
    {
        if (DataManager.Instance.playerData.isWaterFinished = true)
            unlock(1);

        if (DataManager.Instance.playerData.isEarthFinished = true)
            unlock(0);
    }
    

    private void unlock(int index)
    {
        Light[index].SetActive(true);
        InnerTrigger[index].SetActive(true);
        Emblem[index].SetActive(true);
        Glow[index].SetActive(true);
    }
}
