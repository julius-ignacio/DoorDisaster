using System.Collections.Generic;

[System.Serializable]
public class QuestionData
{
    public int id;
    public string question;
    public List<string> options;
    public string correct;
}

[System.Serializable]
public class QuestionListWrapper
{
    public List<QuestionData> questions;
}