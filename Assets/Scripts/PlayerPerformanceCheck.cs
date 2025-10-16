using UnityEngine;

public class PlayerPerformanceCheck : MonoBehaviour
{
    public GameObject warning, barrier;
    
    void Start()
    {
        barrier.SetActive(true);
        warning.SetActive(false);
    }


    void OnTriggerEnter()
    {
        if (DataManager.Instance.Npcs_saved >= 3 && DataManager.Instance.factsDiscovered >= 3)
        {
            barrier.SetActive(false);
        }
        else
        {
            warning.SetActive(true);
        }
    }

}
