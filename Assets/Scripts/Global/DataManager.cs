using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Example stats
    public int playerScore_erudition; 
    public int remainingHealthPoints;
    public int totalQuestionsAnswered;
    public float timeTaken;
    public Dictionary<int, int> npcScores = new Dictionary<int, int>();



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
