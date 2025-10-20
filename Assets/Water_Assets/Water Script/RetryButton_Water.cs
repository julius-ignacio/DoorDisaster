using UnityEngine;
using UnityEngine.SceneManagement;

public class RetryButton_Water : MonoBehaviour
{
    public void RetryLevel_Water()
    {
        SceneManager.LoadScene("Scenes/WATER/Water Stage 1");        // Must match the scene name
    }
}
