using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterTrial : MonoBehaviour
{
    public int trialIndex;
    public GameObject levelSelectUI;


public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optional: stop any currently looping theme before loading new scene
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopAll(); // or StopLoop(), depending on your AudioManager
                AudioManager.Instance.StopAll();
            }

            levelSelectUI.SetActive(true);
            DataManager.Instance.currentTrial = trialIndex;
        }

        else
        {
            levelSelectUI.SetActive(false);
        }
    }

}