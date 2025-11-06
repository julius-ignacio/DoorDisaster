using UnityEngine;
using UnityEngine.SceneManagement;


public class GoToGameMenu : MonoBehaviour
{
    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameMenu");
    }
}
