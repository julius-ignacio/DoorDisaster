using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuizQuestion
{
    public string question;
    public string[] choices;
    public int correctIndex;
}

public enum QuizGroup
{
    // NPCs
    NPC1, NPC2, NPC3, NPC4, NPC5,
    // Medkit
    Medkit1, Medkit2, Medkit3, Medkit4,
    // Water
    Water1, Water2, Water3, Water4,
    // Items
    Whistle1,
    SafetyHelmet
}

public static class QuizDatabase
{
    // Bank[group][mode] => List<QuizQuestion>
    private static readonly Dictionary<QuizGroup, List<QuizQuestion>[]> Bank = new()
    {
        // NPC1
        [QuizGroup.NPC1] = new[]
        {
            // Normal (mode 0)
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "What is the safest position during an earthquake if you’re indoors?",
                    choices = new[] { "Run outside", "Stand near a window", "Drop, Cover, Hold On" },
                    correctIndex = 2
                },
                new QuizQuestion {
                    question = "Where is the safest place to take cover during an earthquake?",
                    choices = new[] { "Near glass windows", "Under a sturdy table", "Next to tall shelves" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "What should you avoid using during or right after an earthquake?",
                    choices = new[] { "The stairs", "The elevator", "The doorway" },
                    correctIndex = 1
                }
            },
            // Hard (mode 1) — scenario-based, trickier distractors
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "Shaking starts and you’re in a room with exterior windows and interior walls. Where should you shelter?",
                    choices = new[] {
                        "In a doorway under the doorframe",
                        "Under sturdy furniture away from exterior windows",
                        "Against an exterior wall below a window"
                    },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "You’re several steps from a desk when strong shaking begins. What’s the safest first action?",
                    choices = new[] {
                        "Run for the exit immediately",
                        "Drop where you are and protect your head/neck",
                        "Stand in a corner until shaking stops"
                    },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "Shaking stops. Fire alarms are blaring and lights are out. What should you do next?",
                    choices = new[] {
                        "Use the elevator to evacuate quickly",
                        "Check for hazards (fire, gas, broken glass) and prepare for aftershocks",
                        "Jump from the nearest window if on a low floor"
                    },
                    correctIndex = 1
                }
            }
        },

        // NPC2
        [QuizGroup.NPC2] = new[]
        {
            // Normal
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "What should you do if shaking starts while cooking?",
                    choices = new[] { "Hide under the table first", "Turn off the stove immediately", "Run outside" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "What should you protect first during an earthquake?",
                    choices = new[] { "Your arms", "Your legs", "Your head and neck" },
                    correctIndex = 2
                },
                new QuizQuestion {
                    question = "Where should you not hide during an earthquake?",
                    choices = new[] { "Away from windows", "Next to heavy cabinets", "Under a sturdy table" },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "You’re in a kitchen when strong shaking starts. What is the safest sequence?",
                    choices = new[] {
                        "Run outside, then turn off gas later",
                        "Drop, cover, hold; when safe, turn off heat sources; evacuate",
                        "Turn off the main electric breaker during shaking"
                    },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "Which location is most dangerous to remain near during shaking?",
                    choices = new[] {
                        "Under a sturdy desk",
                        "Beside tall, unsecured shelving",
                        "In the center of an open classroom"
                    },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "After the main shock, what must you do before moving through hallways?",
                    choices = new[] {
                        "Use elevators to avoid crowded stairs",
                        "Check for falling hazards and glass; expect aftershocks",
                        "Run quickly to avoid aftershocks"
                    },
                    correctIndex = 1
                }
            }
        },

        // NPC3
        [QuizGroup.NPC3] = new[]
        {
            // Normal
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "If you are in bed during an earthquake, what should you do?",
                    choices = new[] { "Hide under the bed", "Stay in bed and protect your head with a pillow", "Run outside" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "After an earthquake, what’s the first thing you should do?",
                    choices = new[] { "Turn on all lights", "Check yourself and others for injuries", "Use the elevator to leave" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "Where should you evacuate after the shaking stops?",
                    choices = new[] { "Near tall power lines", "Inside a car park", "An open area away from buildings" },
                    correctIndex = 2
                }
            },
            // Hard
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "You’re in bed when glass begins to shatter from a nearby window during shaking. Best action?",
                    choices = new[] {
                        "Stay in bed, cover head/neck with a pillow, face away from the window",
                        "Run to the doorway immediately",
                        "Stand up and try to hold the window closed"
                    },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "After shaking stops at night, you smell gas. What should you do first?",
                    choices = new[] {
                        "Flip on lights to see hazards clearly",
                        "Avoid electrical switches, get everyone out, and call the gas company",
                        "Open the oven to ventilate"
                    },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "A strong aftershock is likely. Where do you wait if you can’t evacuate yet?",
                    choices = new[] {
                        "Under sturdy furniture away from windows",
                        "In a stairwell to leave faster",
                        "In an elevator held at your floor"
                    },
                    correctIndex = 0
                }
            }
        },

        // NPC4
        [QuizGroup.NPC4] = new[]
        {
            // Normal
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "What should you do if you are outside during an earthquake?",
                    choices = new[] { "Hide near tall trees", "Move to an open area away from buildings", "Run inside a house" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "If you’re driving during an earthquake, what's the safest action?",
                    choices = new[] { "Stop under a bridge", "Stop in a clear area and stay in the vehicle", "Keep driving fast" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "What should you prepare before an earthquake happens?",
                    choices = new[] { "Party decorations", "An emergency kit", "Extra mirrors" },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "You’re outdoors on a city street when shaking starts. Best immediate action?",
                    choices = new[] {
                        "Move away from buildings, trees, streetlights, and power lines to an open area",
                        "Stand next to a building to avoid traffic",
                        "Shelter under the nearest balcony"
                    },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "While driving on a highway during a quake, what is safest?",
                    choices = new[] {
                        "Stop on the shoulder away from bridges/overpasses and stay in the vehicle",
                        "Brake under an overpass to protect from debris",
                        "Speed up to get out of the area"
                    },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "You feel a long, strong quake near the coast. What should you assume and do?",
                    choices = new[] {
                        "Assume a tsunami risk and move to high ground/inland immediately",
                        "Wait for an official alert before moving",
                        "Go to the beach to observe the water level"
                    },
                    correctIndex = 0
                }
            }
        },

        // NPC5
        [QuizGroup.NPC5] = new[]
        {
            // Normal
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "What is the safest position during an earthquake if you’re indoors?",
                    choices = new[] { "Drop, Cover, Hold On", "Run outside", "Stand near a window" },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "Where is the safest place to take cover during an earthquake?",
                    choices = new[] { "Next to tall shelves", "Under a sturdy table", "Near glass windows" },
                    correctIndex = 1
                },
                new QuizQuestion {
                    question = "What should you avoid using during or right after an earthquake?",
                    choices = new[] { "The elevator", "The doorway", "The stairs" },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion>
            {
                new QuizQuestion {
                    question = "Which is NOT recommended as shelter in modern buildings during a quake?",
                    choices = new[] { "Doorway", "Under sturdy furniture", "Away from exterior windows" },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "You’re in a crowded room. Best practice during shaking is to:",
                    choices = new[] {
                        "Drop, cover, and hold; warn others; shield head/neck",
                        "Run for the exit to avoid being trapped",
                        "Stand in the middle and wave for help"
                    },
                    correctIndex = 0
                },
                new QuizQuestion {
                    question = "After shaking stops, which exit path is safest?",
                    choices = new[] {
                        "Glass‑lined hallway with visible shards",
                        "Stairwell that appears clear of debris (avoid elevators)",
                        "Elevator next to the lobby"
                    },
                    correctIndex = 1
                }
            }
        },

        // Medkit1..4
        [QuizGroup.Medkit1] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Why is it important to include a medkit in your emergency preparedness plan?",
                    choices = new[] {
                        "Because it guarantees survival in all disaster situations",
                        "Because it replaces the need for professional medical care",
                        "Because it can provide immediate first aid before professional help arrives"
                    },
                    correctIndex = 2
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "For severe bleeding control in your medkit, which item is most critical?",
                    choices = new[] { "Tourniquet", "Tweezers", "Thermometer" },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup.Medkit2] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "In which situation would a medkit be most valuable during a disaster?",
                    choices = new[] {
                        "When you need food and water during a prolonged evacuation",
                        "When someone suffers a minor injury and immediate care is needed before professionals arrive",
                        "When trying to predict when rescuers will arrive"
                    },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "A victim has a large object embedded in the arm. What’s the correct first‑aid step?",
                    choices = new[] {
                        "Remove the object quickly, then bandage",
                        "Stabilize the object with padding/bandage and seek medical help",
                        "Apply ice directly to the wound"
                    },
                    correctIndex = 1
                }
            }
        },
        [QuizGroup.Medkit3] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Which of the following items should always be found in a well-prepared medkit?",
                    choices = new[] {
                        "Chargers, gadgets, and batteries",
                        "Adhesive bandages, antiseptic wipes, and pain relievers",
                        "Snacks, bottled water, and a flashlight"
                    },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "A teammate twists an ankle during evacuation. Which medkit item is most appropriate?",
                    choices = new[] {
                        "Elastic (ACE) bandage and cold pack",
                        "Antibiotic ointment",
                        "Burn gel"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup.Medkit4] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "How often should you check or update your medkit supplies?",
                    choices = new[] {
                        "At least once every six months or before the start of disaster season",
                        "Only after using it in an emergency",
                        "There is no need to check it regularly"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "When inspecting your medkit, which items must be replaced immediately?",
                    choices = new[] {
                        "Any item past expiration or with damaged/contaminated packaging",
                        "Only used items",
                        "Only broken plastic containers"
                    },
                    correctIndex = 0
                }
            }
        },

        // Water1..4
        [QuizGroup.Water1] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Why is it important to include water bottles in your emergency kit?",
                    choices = new[] {
                        "Because water bottles can be used as toys to reduce boredom",
                        "Because the human body needs water to survive and dehydration can occur quickly",
                        "Because water bottles can replace the need for medical supplies"
                    },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Recommended minimum water storage per person for emergencies is:",
                    choices = new[] {
                        "One gallon (~3.8 L) per person per day for at least three days",
                        "Half a liter per person per day",
                        "Two cups per person per day"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup.Water2] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "If you have a limited number of water bottles during an evacuation, what should be your top priority for their use?",
                    choices = new[] {
                        "Washing clothes to stay clean",
                        "Putting out small fires",
                        "Drinking, since staying hydrated is critical for survival"
                    },
                    correctIndex = 2
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "With very limited water, priority distribution should go FIRST to:",
                    choices = new[] {
                        "Drinking for children, pregnant people, elderly, and those ill",
                        "Hand washing for the whole group",
                        "Cooking pasta to keep energy up"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup.Water3] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "How much water should each person store for emergency situations?",
                    choices = new[] {
                        "At least one gallon (around 4 liters) per person per day for at least three days",
                        "As much as can fit in a single bottle",
                        "Half a liter per person for the entire emergency"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "If your bottled water runs out, the safest household purification method is:",
                    choices = new[] {
                        "Bring water to a rolling boil for at least 1 minute (3 minutes at high altitude)",
                        "Add any amount of bleach until it smells clean",
                        "Filter through a T‑shirt and drink"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup.Water4] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "What should you do if your bottled water supply runs out during a disaster?",
                    choices = new[] {
                        "Avoid drinking anything until rescuers arrive",
                        "Drink any available water immediately without checking it",
                        "Boil or purify available water sources before drinking"
                    },
                    correctIndex = 2
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Which sign most strongly indicates water is unsafe and should be treated before use?",
                    choices = new[] {
                        "It looks cloudy or has an unusual odor",
                        "It is cold to the touch",
                        "It was collected in the morning"
                    },
                    correctIndex = 0
                }
            }
        },

        // Items
        [QuizGroup.Whistle1] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Why is carrying a whistle important during an emergency or disaster?",
                    choices = new[] {
                        "It helps rescuers locate you by sound even if you're trapped or out of sight",
                        "It can be used to scare away wild animals or intruders",
                        "It's a convenient tool for giving commands to others"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Which whistle pattern is commonly recognized as a distress signal?",
                    choices = new[] {
                        "Three short blasts, repeated",
                        "One long blast every minute",
                        "Continuous rapid blasts without pause"
                    },
                    correctIndex = 0
                }
            }
        },

        [QuizGroup.SafetyHelmet] = new[]
        {
            // Normal
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Why is wearing a safety helmet important during an earthquake?",
                    choices = new[] {
                        "It makes you look prepared and professional during emergencies",
                        "It helps you move faster when evacuating a building",
                        "It protects your head from falling debris and other hazards"
                    },
                    correctIndex = 2
                }
            },
            // Hard
            new List<QuizQuestion> {
                new QuizQuestion {
                    question = "Best practice for helmet use during/after a quake is to:",
                    choices = new[] {
                        "Keep it on during aftershocks; chin strap snug; avoid areas with overhead hazards",
                        "Remove it after the main shock so you can hear better",
                        "Loosen the strap so the helmet can fall off if necessary"
                    },
                    correctIndex = 0
                }
            }
        },
    };

    // Get list for a specific group and mode (0 Normal, 1 Hard)
    public static List<QuizQuestion> Get(QuizGroup group, int mode)
    {
        mode = Mathf.Clamp(mode, 0, 1);
        return Bank[group][mode];
    }

    // Get list for the current mode from DataManager
    public static List<QuizQuestion> Get(QuizGroup group)
    {
        int mode = DataManager.Instance != null ? DataManager.Instance.currentMode : 0;
        mode = Mathf.Clamp(mode, 0, 1);
        return Bank[group][mode];
    }

    // Convenience: get one random question (optionally excluding already-used indices you track elsewhere)
    public static QuizQuestion GetRandom(QuizGroup group, System.Random rng = null)
    {
        var list = Get(group);
        if (list == null || list.Count == 0) return null;
        int i = rng == null ? UnityEngine.Random.Range(0, list.Count) : rng.Next(list.Count);
        return list[i];
    }
}