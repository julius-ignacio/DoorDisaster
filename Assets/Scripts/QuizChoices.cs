using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion
{
    public string question;
    public string[] choices; // store all choices in an array
    public int correctIndex; // optional: which choice is correct
}

public class QuizChoices
{
    public List<QuizQuestion> questions = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "What is the safest position during an earthquake if you’re indoors?",
            choices = new string[] { "Run outside", "Drop, Cover, Hold On", "Stand near a window" },
            correctIndex = 1
        },
        new QuizQuestion {
            question = "Where is the safest place to take cover during an earthquake?",
            choices = new string[] { "Under a sturdy table or desk", "Near glass windows", "Next to tall bookshelves" },
            correctIndex = 0
        },
        new QuizQuestion {
            question = "What should you avoid using during or right after an earthquake?",
            choices = new string[] { "The elevator", "Stairs", "Doorways" },
            correctIndex = 0
        }
    };
}

