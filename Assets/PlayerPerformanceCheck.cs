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
        if (DataManager.Instance.quizScore != 0 && DataManager.Instance.factsDiscovered >= 2)
        {
            barrier.SetActive(false);
        }
        else
        {
            warning.SetActive(true);
        }
    }

}
