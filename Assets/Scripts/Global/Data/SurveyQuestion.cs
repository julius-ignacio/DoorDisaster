using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SurveyQuestion
{
    public string question;
    public string[] choices;
}

public static class SurveyDatabase
{
    public static List<SurveyQuestion> playerFeedback = new List<SurveyQuestion>
    {
        new SurveyQuestion {
            question = "How fun did you find this game?",
            choices = new string[] { "Very fun", "Somewhat fun", "Not fun" }
        },
        new SurveyQuestion {
            question = "Was the game easy to understand?",
            choices = new string[] { "Yes", "A little confusing", "No" }
        },
        new SurveyQuestion {
            question = "Would you recommend this to a friend?",
            choices = new string[] { "Definitely", "Maybe", "No" }
        }
    };
}
