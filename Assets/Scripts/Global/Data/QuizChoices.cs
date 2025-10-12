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
            question = "What is the safest position during an earthquake if youre indoors?5",
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



    // Medkit questions
    public static List<QuizQuestion> Medkit = new List<QuizQuestion>
    {
        new QuizQuestion {
            question = "Why is it important to include a medkit in your emergency preparedness plan?",
            choices = new string[] { "Because it can provide immediate first aid before professional help arrives",
    "Because it guarantees survival in all disaster situations",
    "Because it replaces the need for professional medical care"  },
            correctIndex = 0
        }
   };


    public static List<QuizQuestion> Medkit2 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "In which situation would a medkit be most valuable during a disaster?",
        choices = new string[] {
            "When someone suffers a minor injury and immediate care is needed before professionals arrive",
            "When you need food and water during a prolonged evacuation",
            "When trying to predict when rescuers will arrive"
        },
        correctIndex = 0
    }
};



    public static List<QuizQuestion> Medkit3 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "In which situation would a medkit be most valuable during a disaster?",
        choices = new string[] {
            "When someone suffers a minor injury and immediate care is needed before professionals arrive",
            "When you need food and water during a prolonged evacuation",
            "When trying to predict when rescuers will arrive"
        },
        correctIndex = 0
    }
};

    public static List<QuizQuestion> Water1 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Why is it important to include water bottles in your emergency kit?",
        choices = new string[] {
            "Because the human body needs water to survive and dehydration can occur quickly",
            "Because water bottles can be used as toys to reduce boredom",
            "Because water bottles can replace the need for medical supplies"
        },
        correctIndex = 0
    }
};


    public static List<QuizQuestion> Water2 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "If you have a limited number of water bottles during an evacuation, what should be your top priority for their use?",
        choices = new string[] {
            "Drinking, since staying hydrated is critical for survival",
            "Washing clothes to stay clean",
            "Putting out small fires"
        },
        correctIndex = 0
    }
};

    public static List<QuizQuestion> Water3 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "If you have a limited number of water bottles during an evacuation, what should be your top priority for their use?",
        choices = new string[] {
            "Drinking, since staying hydrated is critical for survival",
            "Washing clothes to stay clean",
            "Putting out small fires"
        },
        correctIndex = 0
    }





};

    public static List<QuizQuestion> Whistle1 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Why is carrying a whistle important during an emergency or disaster?",
        choices = new string[] {
            "It helps rescuers locate you by sound even if you're trapped or out of sight",
            "It can be used to scare away wild animals or intruders",
            "It's a convenient tool for giving commands to others"
        },
        correctIndex = 0
    }
};

public static List<QuizQuestion> SafetyHelmet = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Why is wearing a safety helmet important during an earthquake?",
        choices = new string[] {
            "It makes you look prepared and professional during emergencies",
            "It helps you move faster when evacuating a building",
            "It protects your head from falling debris and other hazards"
        },
        correctIndex = 2
    }
};







}

