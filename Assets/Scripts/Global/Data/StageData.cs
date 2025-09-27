using System.Collections.Generic;


[System.Serializable]
public class StageData
{
    public int quizScore;
    public int questionsAnswered;
    public int factsDiscovered;
    public int totalScore; // quizScore + factsDiscovered
     public static StageData Instance;



        // NEW: NPC scores per stage
    public Dictionary<int, int> npcScores = new Dictionary<int, int>();
}