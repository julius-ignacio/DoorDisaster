using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterTrial : MonoBehaviour
{
    public string sceneName;
    public int trialIndex;


public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Optional: stop any currently looping theme before loading new scene
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopAll(); // or StopLoop(), depending on your AudioManager
            }

            DataManager.Instance.currentTrial = trialIndex;
            SceneManager.LoadScene(sceneName);
        }
    }

}
