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
    public int overallTotalScore;
    public int totalQuestionsAnswered;
    public string lastPlayedScene;
}
