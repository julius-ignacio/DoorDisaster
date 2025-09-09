using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Example stats
    public int playerScore; 
    public int remainingHealthPoints;
    public int totalQuestionsAnswered;
    public float timeTaken;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keeps data across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
