using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataManager_Water : MonoBehaviour
{
    public static DataManager_Water Instance { get; private set; }

    [Header("Progress Tracking")]
    public int factsDiscovered = 0;
    public int quizScore = 0;     // ✅ Correct Answers
    public int wrongAnswers = 0;  // ❌ Wrong Answers
    public float totalPlayTime = 0f;

    [Header("Collected Items / Keys")]
    public HashSet<string> collectedItems = new HashSet<string>();
    public HashSet<string> collectedKeys = new HashSet<string>();

    [Header("Scene Tracking")]
    public string currentSceneName = "";

    [Header("UI Reference")]
    [Tooltip("Assign a TextMeshProUGUI object in Canvas to show Data info")]
    public TextMeshProUGUI dataText;

    // 👇 Computed Properties
    public int totalQuestionsAnswered => quizScore + wrongAnswers;
    public int PlayerTotalScore => quizScore + factsDiscovered;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 🔧 If no UI assigned, try to find one automatically
        if (dataText == null)
        {
            TextMeshProUGUI foundText = FindObjectOfType<TextMeshProUGUI>();
            if (foundText != null)
            {
                dataText = foundText;
                Debug.Log("[DataManager] Automatically found a TextMeshProUGUI for data display.");
            }
            else
            {
                Debug.LogWarning("[DataManager] No TextMeshProUGUI assigned or found. Please assign one in Inspector!");
            }
        }
    }

    private void Update()
    {
        totalPlayTime += Time.deltaTime;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (dataText == null)
            return;

        // ✅ Only show main player data
        string info =
            $"<b>PLAYER DATA</b>\n" +
            $"Facts Discovered: {factsDiscovered}\n" +
            $"Quiz Score: {quizScore}\n" +
            $"Wrong Answers: {wrongAnswers}\n" +
            $"Total Play Time: {totalPlayTime:F1}s\n" +
            $"Total Questions Answered: {totalQuestionsAnswered}\n" +
            $"Player Total Score: {PlayerTotalScore}";

        dataText.text = info;
    }

    public void AddQuizScore(int amount = 1)
    {
        if (amount <= 0) return;
        quizScore += amount;
        Debug.Log($"[DataManager] ✅ Correct +{amount} (Total Correct: {quizScore})");
    }

    public void AddWrongAnswer(int amount = 1)
    {
        if (amount <= 0) return;
        wrongAnswers += amount;
        Debug.Log($"[DataManager] ❌ Wrong +{amount} (Total Wrong: {wrongAnswers})");
    }

    public void AddFact(string factId)
    {
        if (string.IsNullOrEmpty(factId)) return;
        if (collectedItems.Add($"fact:{factId}"))
            factsDiscovered++;
    }

    public void CollectItem(string itemId)
    {
        if (!string.IsNullOrEmpty(itemId))
            collectedItems.Add(itemId);
    }

    public void CollectKey(string keyId)
    {
        if (!string.IsNullOrEmpty(keyId))
            collectedKeys.Add(keyId);
    }

    public bool HasItem(string itemId) => collectedItems.Contains(itemId);
    public bool HasKey(string keyId) => collectedKeys.Contains(keyId);

    public void ResetData()
    {
        factsDiscovered = 0;
        quizScore = 0;
        wrongAnswers = 0;
        totalPlayTime = 0f;
        collectedItems.Clear();
        collectedKeys.Clear();
        currentSceneName = "";
        UpdateUI();
    }
}
