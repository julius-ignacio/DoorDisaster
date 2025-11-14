using UnityEngine;

[System.Serializable]
public class PlayerData
{
    // Profile
    public string playerId;
    public string playerName;
    public string email;
    public int age;
    public int gradeLevel;

    [Header("Normal / Hard")]
    public ModeData[] Mode = new ModeData[2];

    // Game progress
    public bool isSurveyDone, isFireFinished, isWaterFinished, isEarthFinished;

    // ✅ Tutorial completion flag
    public bool tutorialDone = false;

    public PlayerData()
    {
        Mode = new ModeData[2] { new ModeData(), new ModeData() };
    }
}
