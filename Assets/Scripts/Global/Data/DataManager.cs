using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // 🔹 New structured data for DB
    public PlayerData playerData = new PlayerData();

    // 🔹 Legacy/global fields (for quick access in scripts)
    public int quizScore;
    public int factsDiscovered;
    public int totalQuestionsAnswered;
    public int Npcs_saved;

    public Dictionary<int, int> npcScores = new Dictionary<int, int>();


    [Header("Current Trial")]
    public int currentTrial;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public void InitPlayerData()
    {
        if (playerData == null)
            playerData = new PlayerData();

        if (string.IsNullOrEmpty(playerData.playerId))
        {
            playerData.playerId = FirebaseAuth.UserLocalId;
            playerData.playerName = "Player";
            playerData.trials = new TrialData[3];

            for (int i = 0; i < playerData.trials.Length; i++)
            {
                playerData.trials[i] = new TrialData();
            }

        }
    }

public void SaveTrialData(int trialIndex)
{
    var trial = playerData.trials[trialIndex];

    trial.quizScore = quizScore;
    trial.questionsAnswered = totalQuestionsAnswered;
    trial.factsDiscovered = factsDiscovered;
    trial.totalScore = trial.quizScore + trial.factsDiscovered;

    // ✅ Set the correct finished flag
    switch (currentTrial)
    {
        case 0:
            playerData.isEarthFinished = true;
            break;

        case 1:
            playerData.isWaterFinished = true;
            break;

        case 2:
            playerData.isFireFinished = true;
            break;
    }

    UpdateTotals();
}



    private void UpdateTotals()
    {
        int overall = 0;
        int totalQuestions = 0;


        playerData.overallTotalScore = overall;
        playerData.totalQuestionsAnswered = totalQuestions;
    }

}
