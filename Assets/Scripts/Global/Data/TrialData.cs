using System.Collections.Generic;


[System.Serializable]
public class TrialData
{
    public int quizScore;
    public int wrongAnswers;
    public int questionsAnswered;
    public int factsDiscovered;
    public int totalScore; // quizScore + factsDiscovered
}