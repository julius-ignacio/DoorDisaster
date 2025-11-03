[System.Serializable]
public class PlayerData
{
    // Profile
    public string playerId;
    public string playerName;
    public string email;
    public int age;
    public int gradeLevel;

    // Game progress
    public TrialData[] trials = new TrialData[3];
    public bool isEarthFinished, isWaterFinished, isFireFinished, isSurveyDone;
    public bool isVerified;
}
