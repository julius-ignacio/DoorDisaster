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
        choices = new string[] { "Run outside", "Stand near a window", "Drop, Cover, Hold On" },
        correctIndex = 2
    },
    new QuizQuestion {
        question = "Where is the safest place to take cover during an earthquake?",
        choices = new string[] { "Near glass windows", "Under a sturdy table", "Next to tall shelves" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "What should you avoid using during or right after an earthquake?",
        choices = new string[] { "The stairs", "The elevator", "The doorway" },
        correctIndex = 1
    }
};

// NPC2 questions
public static List<QuizQuestion> NPC2 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "What should you do if shaking starts while cooking?",
        choices = new string[] { "Hide under the table first", "Turn off the stove immediately", "Run outside" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "What should you protect first during an earthquake?",
        choices = new string[] { "Your arms", "Your legs", "Your head and neck" },
        correctIndex = 2
    },
    new QuizQuestion {
        question = "Where should you not hide during an earthquake?",
        choices = new string[] { "Away from windows", "Next to heavy cabinets", "Under a sturdy table" },
        correctIndex = 1
    }
};

// NPC3 questions
public static List<QuizQuestion> NPC3 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "If you are in bed during an earthquake, what should you do?",
        choices = new string[] { "Hide under the bed", "Stay in bed and protect your head with a pillow", "Run outside" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "After an earthquake, what’s the first thing you should do?",
        choices = new string[] { "Turn on all lights", "Check yourself and others for injuries", "Use the elevator to leave" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "Where should you evacuate after the shaking stops?",
        choices = new string[] { "Near tall power lines", "Inside a car park", "An open area away from buildings" },
        correctIndex = 2
    }
};

// NPC4 questions
public static List<QuizQuestion> NPC4 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "What should you do if you are outside during an earthquake?",
        choices = new string[] { "Hide near tall trees", "Move to an open area away from buildings", "Run inside a house" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "If you’re driving during an earthquake, what's the safest action?",
        choices = new string[] { "Stop under a bridge", "Stop in a clear area and stay in the vehicle", "Keep driving fast" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "What should you prepare before an earthquake happens?",
        choices = new string[] { "Party decorations", "An emergency kit", "Extra mirrors" },
        correctIndex = 1
    }
};

// NPC5 questions
public static List<QuizQuestion> NPC5 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "What is the safest position during an earthquake if you’re indoors?",
        choices = new string[] { "Drop, Cover, Hold On", "Run outside", "Stand near a window" },
        correctIndex = 0
    },
    new QuizQuestion {
        question = "Where is the safest place to take cover during an earthquake?",
        choices = new string[] { "Next to tall shelves", "Under a sturdy table", "Near glass windows" },
        correctIndex = 1
    },
    new QuizQuestion {
        question = "What should you avoid using during or right after an earthquake?",
        choices = new string[] { "The elevator", "The doorway", "The stairs" },
        correctIndex = 0
    }
};



    // Medkit questions
public static List<QuizQuestion> Medkit = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Why is it important to include a medkit in your emergency preparedness plan?",
        choices = new string[] {
            "Because it guarantees survival in all disaster situations",
            "Because it replaces the need for professional medical care",
            "Because it can provide immediate first aid before professional help arrives"
        },
        correctIndex = 2
    }
};

public static List<QuizQuestion> Medkit2 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "In which situation would a medkit be most valuable during a disaster?",
        choices = new string[] {
            "When you need food and water during a prolonged evacuation",
            "When someone suffers a minor injury and immediate care is needed before professionals arrive",
            "When trying to predict when rescuers will arrive"
        },
        correctIndex = 1
    }
};

public static List<QuizQuestion> Medkit3 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Which of the following items should always be found in a well-prepared medkit?",
        choices = new string[] {
            "Chargers, gadgets, and batteries",
            "Adhesive bandages, antiseptic wipes, and pain relievers",
            "Snacks, bottled water, and a flashlight"
        },
        correctIndex = 1
    }
};

public static List<QuizQuestion> Medkit4 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "How often should you check or update your medkit supplies?",
        choices = new string[] {
            "At least once every six months or before the start of disaster season",
            "Only after using it in an emergency",
            "There is no need to check it regularly"
        },
        correctIndex = 0
    }
};


public static List<QuizQuestion> Water1 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "Why is it important to include water bottles in your emergency kit?",
        choices = new string[] {
            "Because water bottles can be used as toys to reduce boredom",
            "Because the human body needs water to survive and dehydration can occur quickly",
            "Because water bottles can replace the need for medical supplies"
        },
        correctIndex = 1
    }
};

public static List<QuizQuestion> Water2 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "If you have a limited number of water bottles during an evacuation, what should be your top priority for their use?",
        choices = new string[] {
            "Washing clothes to stay clean",
            "Putting out small fires",
            "Drinking, since staying hydrated is critical for survival"
        },
        correctIndex = 2
    }
};

public static List<QuizQuestion> Water3 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "How much water should each person store for emergency situations?",
        choices = new string[] {
            "At least one gallon (around 4 liters) per person per day for at least three days",
            "As much as can fit in a single bottle",
            "Half a liter per person for the entire emergency"
        },
        correctIndex = 0
    }
};

public static List<QuizQuestion> Water4 = new List<QuizQuestion>
{
    new QuizQuestion {
        question = "What should you do if your bottled water supply runs out during a disaster?",
        choices = new string[] {
            "Avoid drinking anything until rescuers arrive",
            "Drink any available water immediately without checking it",
            "Boil or purify available water sources before drinking"
        },
        correctIndex = 2
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

