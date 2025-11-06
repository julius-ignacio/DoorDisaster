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
    public Material LockedTrialMaterial, waterGlow, earthGlow;

    [Header("Lights")]
    public GameObject[] Light;

    [Header("Vines")]
    public GameObject Vine1, Vine2;

    void Start()
    {
        Light[0].SetActive(false);
        Light[1].SetActive(false);

        Vine1.SetActive(true);
        Vine2.SetActive(true);

        InnerTrigger[0].SetActive(false);
        InnerTrigger[1].SetActive(false);

        Glow[0].GetComponent<Renderer>().material = LockedTrialMaterial;
        Glow[1].GetComponent<Renderer>().material = LockedTrialMaterial;

        Emblem[0].GetComponent<Renderer>().material = LockedTrialMaterial;
        Emblem[1].GetComponent<Renderer>().material = LockedTrialMaterial;
    }
    void Update()
    {
        if (DataManager.Instance.playerData.isWaterFinished == true)
            unlock(0); //unlocks earth
        else
        { lockTrial(0); }//locks earth

        if (DataManager.Instance.playerData.isFireFinished == true)
            unlock(1);//unlocks water
        else
        { lockTrial(1); } //locks water


    }


    private void unlock(int index)
    {
        Light[index].SetActive(true);
        InnerTrigger[index].SetActive(true);
        switch (index)
        {
            case 0:
                Glow[index].GetComponent<Renderer>().material = earthGlow;
                Emblem[index].GetComponent<Renderer>().material = earthGlow;
                Vine1.SetActive(false);
                

                break;

            case 1:
                Glow[index].GetComponent<Renderer>().material = waterGlow;
                Emblem[index].GetComponent<Renderer>().material = waterGlow;
                Vine2.SetActive(false);


                break;
        }
    }
    

        private void lockTrial(int index)
    {
        Light[index].SetActive(false);
        InnerTrigger[index].SetActive(false);
        switch (index)
        {
            case 0:
                Glow[index].GetComponent<Renderer>().material = LockedTrialMaterial;
                Emblem[index].GetComponent<Renderer>().material = LockedTrialMaterial;
                Vine1.SetActive(true);
                break;

            case 1:
                Glow[index].GetComponent<Renderer>().material = LockedTrialMaterial;
                Emblem[index].GetComponent<Renderer>().material = LockedTrialMaterial;
                Vine2.SetActive(true);
                break;
        }
    }
}
