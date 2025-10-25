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
            question = "I enjoyed playing this game.",
            choices = new string[] { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
        },
        new SurveyQuestion {
            question = "The game mechanics were easy to understand.",
            choices = new string[] { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
        },
        new SurveyQuestion {
            question = "The visuals and graphics were appealing.",
            choices = new string[] { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
        },
        new SurveyQuestion {
            question = "The controls were responsive and easy to use.",
            choices = new string[] { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
        },
        new SurveyQuestion {
            question = "I would recommend this game to others.",
            choices = new string[] { "Strongly Agree", "Agree", "Neutral", "Disagree", "Strongly Disagree" }
        }
    };
}
