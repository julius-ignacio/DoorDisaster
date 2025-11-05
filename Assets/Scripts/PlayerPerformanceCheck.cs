using MilkShake;
using UnityEngine;

public class PlayerPerformanceCheck : MonoBehaviour
{
    public GameObject warning, barrier, DestroyBtn3, trigger;
    private bool isObjectivesCompleted = false;

    public ConsistentQuake consistentQuake;
    public Shaker shake;
    public GameObject panicmeterUI;
    public PanicMeterScript panicMeterScript;
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
            trigger.SetActive(false);
            DestroyBtn3.SetActive(false);


                    // Audio switching logic (this part always runs)
        AudioClip temp = AudioManager.Instance.Clips[2];
        AudioManager.Instance.Clips[2] = AudioManager.Instance.Clips[1];
        AudioManager.Instance.Clips[1] = temp;

        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[20]);
            AudioManager.Instance.audLoop.volume = 0.5f;
        

                // Safely handle quake + shake
        if (shake != null)
            shake.enabled = false;

        if (consistentQuake != null)
        {
            consistentQuake.enabled = false;
            consistentQuake.PauseQuakes();
        }

               if (panicMeterScript != null)
        {
            panicMeterScript.currHealth = 0;
            panicmeterUI.SetActive(false);
        }
        }
        else
        {
            warning.SetActive(true);
        }
    }
}
