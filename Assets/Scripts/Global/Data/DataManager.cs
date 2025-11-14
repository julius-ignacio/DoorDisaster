using System.Collections.Generic;
using UnityEngine;
using System.IO;

[DefaultExecutionOrder(-100)]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public PlayerData playerData = new PlayerData();

    public int quizScore;
    public int wrongAnswers;
    public int factsDiscovered;
    public int totalQuestionsAnswered;
    public int Npcs_saved;

    public Dictionary<int, int> npcScores = new Dictionary<int, int>();

    [Header("Current Trial Info")]
    public int currentTrial; // 0 = Fire, 1 = Water, 2 = Earth
    public int currentMode;  // 0 = Normal, 1 = Hard

    [HideInInspector] public bool skipNextWorldLoad = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved data on startup
            LoadPlayerDataFromDisk();
            LoadGlobalProgress();

            InitPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Auto-save when app closes
    private void OnApplicationQuit()
    {
        Debug.Log("App closing - saving data...");
        SavePlayerDataToDisk();
        SaveGlobalProgress();
    }

    // Auto-save when app pauses (CRITICAL FOR MOBILE!)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Debug.Log("App paused - saving data...");
            SavePlayerDataToDisk();
            SaveGlobalProgress();
        }
    }

    // Save PlayerData to JSON file
    private void SavePlayerDataToDisk()
    {
        string json = JsonUtility.ToJson(playerData, true);
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");
        File.WriteAllText(path, json);
        Debug.Log($"PlayerData saved to {path}");
    }

    // Load PlayerData from JSON file
    private void LoadPlayerDataFromDisk()
    {
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("✅ PlayerData loaded from disk");
        }
        else
        {
            Debug.Log("No saved PlayerData found - starting fresh");
        }
    }

    // Save current trial/mode selection
    public void SaveGlobalProgress()
    {
        PlayerPrefs.SetInt("CurrentMode", currentMode);
        PlayerPrefs.SetInt("CurrentTrial", currentTrial);
        PlayerPrefs.Save();
    }

    // Load current trial/mode selection
    private void LoadGlobalProgress()
    {
        currentMode = PlayerPrefs.GetInt("CurrentMode", 0);
        currentTrial = PlayerPrefs.GetInt("CurrentTrial", 0);
    }

    public void InitPlayerData()
    {
        if (playerData == null)
            playerData = new PlayerData();

        if (string.IsNullOrEmpty(playerData.playerId))
        {
            playerData.playerId = string.IsNullOrEmpty(FirebaseAuth.UserLocalId) ? "" : FirebaseAuth.UserLocalId;
            playerData.playerName = "Player";

            for (int m = 0; m < playerData.Mode.Length; m++)
            {
                if (playerData.Mode[m] == null)
                    playerData.Mode[m] = new ModeData();

                for (int t = 0; t < playerData.Mode[m].trials.Length; t++)
                {
                    if (playerData.Mode[m].trials[t] == null)
                        playerData.Mode[m].trials[t] = new TrialData();
                }
            }
        }
    }

    public void SaveTrialData(int trialIndex, int modeIndex)
    {
        var trial = playerData.Mode[modeIndex].trials[trialIndex];
        trial.quizScore = quizScore;
        trial.questionsAnswered = totalQuestionsAnswered;
        trial.factsDiscovered = factsDiscovered;
        trial.totalScore = trial.quizScore + trial.factsDiscovered;

        // Also save to disk immediately
        SavePlayerDataToDisk();
    }

    public void LoadTrialStatsIntoGlobals(int trialIndex, int modeIndex)
    {
        var trial = playerData.Mode[modeIndex].trials[trialIndex];
        quizScore = trial.quizScore;
        totalQuestionsAnswered = trial.questionsAnswered;
        factsDiscovered = trial.factsDiscovered;
        wrongAnswers = 0;
        Npcs_saved = 0;
    }

    public void ResetGlobalsForNewRun()
    {
        quizScore = 0;
        wrongAnswers = 0;
        factsDiscovered = 0;
        totalQuestionsAnswered = 0;
        Npcs_saved = 0;
        npcScores.Clear();
    }

    public void BeginTrial(int trialIndex, int modeIndex, bool resetIfNoLocalSave)
    {
        currentTrial = trialIndex;
        currentMode = modeIndex;

        LoadTrialStatsIntoGlobals(trialIndex, modeIndex);

        if (resetIfNoLocalSave && !WorldSaveSystem.HasSaveData(trialIndex, modeIndex))
        {
            ResetGlobalsForNewRun();
        }

        SaveGlobalProgress();
    }
}