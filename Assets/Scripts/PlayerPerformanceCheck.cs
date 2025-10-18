using UnityEngine;

public class PlayerPerformanceCheck : MonoBehaviour
{
    public GameObject warning, barrier, DestroyBtn3;
    private bool isObjectivesCompleted = false;

    void Start()
    {
        barrier.SetActive(true);
        warning.SetActive(false);
        DestroyBtn3.SetActive(false);
    }
    

    void Update()
    {
        if (DataManager.Instance.Npcs_saved == 5 && DataManager.Instance.factsDiscovered >= 3)
        {
            isObjectivesCompleted = true;
        }
        else
        {
            warning.SetActive(true);
        }
    }


    void OnTriggerEnter()
    {
        DestroyBtn3.SetActive(true);
    }



    void OnTriggerExit()
    {
        DestroyBtn3.SetActive(false);
    }

    public void DestroyWall()
    {
        if (isObjectivesCompleted)
        {
            warning.SetActive(false);
            barrier.SetActive(false);
            AudioManager.Instance.PlaySFX(22);
        }

        else
        {
            warning.SetActive(true);
        }
    }

}
