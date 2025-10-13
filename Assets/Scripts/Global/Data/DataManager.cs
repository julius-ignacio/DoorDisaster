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


    [Header("Current Trial and Stage")]
    public int currentTrial;
    public int currentStage;


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
            playerData.trials[i].stages = new StageData[2];
            for (int j = 0; j < 2; j++)
                playerData.trials[i].stages[j] = new StageData();
        }
    }
}


    // 🔹 Called when you want to sync global fields into structured DB
    public void SaveStageData(int trialIndex, int stageIndex)
    {
        var stage = playerData.trials[trialIndex].stages[stageIndex];

        stage.quizScore = quizScore;
        stage.questionsAnswered = totalQuestionsAnswered;
        stage.totalScore = quizScore + stage.factsDiscovered;

        // Copy per-NPC scores
        foreach (var kvp in npcScores)
        {
            stage.npcScores[kvp.Key] = kvp.Value;
        }

        UpdateTotals();
    }

    private void UpdateTotals()
    {
        int overall = 0;
        int totalQuestions = 0;

        foreach (var trial in playerData.trials)
        {
            int trialSum = 0;
            foreach (var stage in trial.stages)
            {
                trialSum += stage.totalScore;
                totalQuestions += stage.questionsAnswered;
            }
            trial.trialTotalScore = trialSum;
            overall += trialSum;
        }

        playerData.overallTotalScore = overall;
        playerData.totalQuestionsAnswered = totalQuestions;
    }
}
