[System.Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;

    public TrialData[] trials = new TrialData[3]; // 3 trials in total
    public int overallTotalScore; // sum of all trials

    // Optional meta
    public int totalQuestionsAnswered;
    public string lastPlayedScene;
}