using UnityEngine;

public class PlayerPerformanceCheck : MonoBehaviour
{
    public GameObject warning, barrier, DestroyBtn3;
    private bool isObjectivesCompleted = false;

    void Start()
    {
        barrier.SetActive(true);
        warning.SetActive(true);
        DestroyBtn3.SetActive(false);
    }

    void Update()
    {
        if (DataManager.Instance.Npcs_saved >= 3 && DataManager.Instance.factsDiscovered >= 5)
        {
            if (!isObjectivesCompleted)
            {
                isObjectivesCompleted = true;
                warning.SetActive(false); // ✅ Hide warning when objectives are completed
            }
        }
        else
        {
            isObjectivesCompleted = false;
            warning.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DestroyBtn3.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            DestroyBtn3.SetActive(false);
    }

    public void DestroyWall()
    {
        if (isObjectivesCompleted)
        {
            barrier.SetActive(false);
            AudioManager.Instance.PlaySFX(22);
        }
        else
        {
            warning.SetActive(true);
        }
    }
}
