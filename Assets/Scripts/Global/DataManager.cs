using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // Example stats
    public int remainingHealthPoints;
    public int totalQuestionsAnswered;
    public int factsDiscovered;
    public int quizScore;
    public float timeTaken;
    public string scene;

    public int playerTotalScore;

    public Dictionary<int, int> npcScores = new Dictionary<int, int>();

    void Update()
    {
        playerTotalScore = quizScore + factsDiscovered;
    }


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
