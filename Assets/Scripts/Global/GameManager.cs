using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerNumbers player = new PlayerNumbers();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
