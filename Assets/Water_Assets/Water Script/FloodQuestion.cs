using System;

[Serializable]
public class FloodQuestion
{
    public string question;       // The question text
    public string[] choices;      // Answer choices
    public int correctIndex;      // Index of the correct choice
}
