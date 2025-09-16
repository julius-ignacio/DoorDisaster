using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Example stats
    public int playerScore_erudition; 
    public int remainingHealthPoints;
    public int totalQuestionsAnswered;
    public float timeTaken;
    public int[] individualNpcScores = new int[5]; // Scores for each of the 4 NPCs


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
