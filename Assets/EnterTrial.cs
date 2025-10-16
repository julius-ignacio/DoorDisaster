using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterTrial : MonoBehaviour
{
    public string sceneName;


public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                 SceneManager.LoadScene(sceneName);
        }
    }

}
