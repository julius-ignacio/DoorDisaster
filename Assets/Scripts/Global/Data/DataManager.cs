using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-100)] // initialize before most other scripts
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    // New structured data for DB
    public PlayerData playerData = new PlayerData();

    // Legacy/global fields (for quick access in scripts)
    public int quizScore;
    public int wrongAnswers;
    public int factsDiscovered;
    public int totalQuestionsAnswered;
    public int Npcs_saved;

    public bool isEartFinishedNormal, isWaterFinishedNormal, isFireFinishedNormal;
    public bool isEartFinishedHard, isWaterFinishedHard, isFireFinishedHard;
    public Dictionary<int, int> npcScores = new Dictionary<int, int>();

    [Header("Current Trial Info")]
    public int currentTrial; // 0 = Fire, 1 = Water, 2 = Earth
    public int currentMode;  // 0 = Normal, 1 = Hard

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

     // NEW: consumed by WorldLoader after Restart to skip loading once
    [HideInInspector] public bool skipNextWorldLoad = false;


    public void InitPlayerData()
    {
        if (playerData == null)
            playerData = new PlayerData();

        // Initialize player info if new
        if (string.IsNullOrEmpty(playerData.playerId))
        {
            // Guard: FirebaseAuth may not be set yet; keep empty if so
            playerData.playerId = string.IsNullOrEmpty(FirebaseAuth.UserLocalId) ? "" : FirebaseAuth.UserLocalId;
            playerData.playerName = "Player";

            // Initialize modes (Normal & Hard)
            for (int m = 0; m < playerData.Mode.Length; m++)
            {
                if (playerData.Mode[m] == null)
                    playerData.Mode[m] = new ModeData();

                // Initialize 3 trials for each mode
                for (int t = 0; t < playerData.Mode[m].trials.Length; t++)
                {
                    if (playerData.Mode[m].trials[t] == null)
                        playerData.Mode[m].trials[t] = new TrialData();
                }
            }
        }
    }

void SaveAchievements()
    {
            switch (currentTrial)
    {
        case 0:
                if (currentMode == 0)
                {
                    playerData.isFireFinishedNormal = true;
                }
                else
                {
                    playerData.isFireFinishedHard = true;
                }
            break;


            case 1:
                if (currentMode == 0)
                {
                    playerData.isWaterFinishedNormal = true;
                }
                else
                {
                    playerData.isWaterFinishedHard = true;
                }
            break;

            case 2:
                if (currentMode == 0)
                {
                    playerData.isEarthFinishedNormal = true;
                }
                else
                {
                    playerData.isEarthFinishedHard = true;
                }
            break;

    }
    }

    // Called on pause/quit/end to write globals into PlayerData
    public void SaveTrialData(int trialIndex, int modeIndex)
    {
        var trial = playerData.Mode[modeIndex].trials[trialIndex];
        trial.quizScore = quizScore;
        trial.questionsAnswered = totalQuestionsAnswered;
        trial.factsDiscovered = factsDiscovered;
        trial.totalScore = trial.quizScore + trial.factsDiscovered;


            SaveAchievements();

    }

    // Optional: pull a saved trial’s stats back into the quick-access globals for UI
    public void LoadTrialStatsIntoGlobals(int trialIndex, int modeIndex)
    {
        var trial = playerData.Mode[modeIndex].trials[trialIndex];
        quizScore = trial.quizScore;
        totalQuestionsAnswered = trial.questionsAnswered;
        factsDiscovered = trial.factsDiscovered;
        // wrongAnswers and Npcs_saved are session-scoped; reset or track separately as needed
        wrongAnswers = 0;
        Npcs_saved = 0;
    }

    // Optional: reset globals when starting a brand-new run of a trial
    public void ResetGlobalsForNewRun()
    {
        quizScore = 0;
        wrongAnswers = 0;
        factsDiscovered = 0;
        totalQuestionsAnswered = 0;
        Npcs_saved = 0;
        npcScores.Clear();
    }

    // Optional: single entry to begin a trial, with fallback if no local world save exists
    public void BeginTrial(int trialIndex, int modeIndex, bool resetIfNoLocalSave)
    {
        currentTrial = trialIndex;
        currentMode = modeIndex;

        // If you want the UI to reflect last recorded trial stats immediately:
        LoadTrialStatsIntoGlobals(trialIndex, modeIndex);

        // If there is no local world save and you want a fresh run, reset globals
        if (resetIfNoLocalSave && !WorldSaveSystem.HasSaveData(trialIndex, modeIndex))
        {
            ResetGlobalsForNewRun();
        }
    }
}





