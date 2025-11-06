using System.Collections.Generic;


[System.Serializable]
public class TrialData
{
    public int quizScore;
    public int wrongAnswers;
    public int questionsAnswered;
    public int factsDiscovered;
    public int totalScore; // quizScore + factsDiscovered


    // NEW: run intro only once per trial/mode
    public bool hasSeenIntro;
}