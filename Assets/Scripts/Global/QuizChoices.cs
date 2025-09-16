using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public string[] choices; // store all choices in an array
    public int correctIndex; // optional: which choice is correct

}

public static class QuizDatabase
{
    // NPC1 questions
    public static List<QuizQuestion> NPC1 = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "What is the safest position during an earthquake if you’re indoors?",
            choices = new string[] { "Run outside", "Drop, Cover, Hold On", "Stand near a window" },
            correctIndex = 1
        },
        new QuizQuestion {
            question = "Where is the safest place to take cover during an earthquake?",
            choices = new string[] { "Under a sturdy table", "Near glass windows", "Next to tall shelves" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "What should you avoid using during or right after an earthquake?",
            choices = new string[] { "The elevator", "The stairs", "The doorway" },
            correctIndex = 0
        }
    };

    // NPC2 questions
    public static List<QuizQuestion> NPC2 = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "What should you do if shaking starts while cooking?",
            choices = new string[] { "Turn off the stove immediately", "Run outside", "Hide under the table first" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "What should you protect first during an earthquake?",
            choices = new string[] { "Your legs", "Your head and neck", "Your arms" },
            correctIndex = 1
        },
        new QuizQuestion {
            question = "Where should you not hide during an earthquake?",
            choices = new string[] { "Under a sturdy table", "Next to heavy cabinets", "Away from windows" },
            correctIndex = 1
        }
    };

    // NPC3 questions
    public static List<QuizQuestion> NPC3 = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "If you are in bed during an earthquake, what should you do?",
            choices = new string[] { "Run outside", "Stay in bed and protect your head with a pillow", "Hide under the bed" },
            correctIndex = 1
        },
        new QuizQuestion {
            question = "After an earthquake, what’s the first thing you should do?",
            choices = new string[] { "Check yourself and others for injuries", "Turn on all lights", "Use the elevator to leave" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "Where should you evacuate after the shaking stops?",
            choices = new string[] { "An open area away from buildings", "Inside a car park", "Near tall power lines" },
            correctIndex = 0
        }
    };

    // NPC4 questions
    public static List<QuizQuestion> NPC4 = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "What should you do if you are outside during an earthquake?",
            choices = new string[] { "Move to an open area away from buildings", "Run inside a house", "Hide near tall trees" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "If you’re driving during an earthquake, what's the safest action?",
            choices = new string[] { "Stop in a clear area and stay in the vehicle", "Stop under a bridge", "Keep driving fast" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "What should you prepare before an earthquake happens?",
            choices = new string[] { "An emergency kit", "Extra mirrors", "Party decorations" },
            correctIndex = 0
        }
    };


    // NPC5 questions
    public static List<QuizQuestion> NPC5 = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "What is the safest position during an earthquake if you’re indoors?5",
            choices = new string[] { "Run outside", "Drop, Cover, Hold On", "Stand near a window" },
            correctIndex = 1
        },
        new QuizQuestion {
            question = "Where is the safest place to take cover during an earthquake?5",
            choices = new string[] { "Under a sturdy table", "Near glass windows", "Next to tall shelves" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "What should you avoid using during or right after an earthquake?5",
            choices = new string[] { "The elevator", "The stairs", "The doorway" },
            correctIndex = 0
        }
   };


}

