using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuizQuestion_water
{
    public string question;
    public string[] choices;
    public int correctIndex;
}

public enum QuizGroup_water
{
    // NPCs
    NPC1, NPC2, NPC3, NPC4, NPC5,
    // Medkit
    Medkit1, Medkit2, Medkit3, Medkit4,
    // Water
    Water1, Water2, Water3, Water4,
    // Items
    Whistle1,
    SafetyHelmet,
    Radio1
}

public static class QuizDatabase_water
{
    // Bank[group][mode] => List<QuizQuestion_water>
    private static readonly Dictionary<QuizGroup_water, List<QuizQuestion_water>[]> Bank = new()
    {
        // NPC1 - Flood preparedness (Indoor)
        [QuizGroup_water.NPC1] = new[]
        {
            // Normal (mode 0)
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "If a flood warning is issued and you are indoors on the ground floor, what should you do first?",
                    choices = new[] { "Stay and wait for rescuers to arrive", "Move to higher ground or an upper floor immediately", "Open all windows for ventilation" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Which household item is most important to secure before floodwaters rise?",
                    choices = new[] { "Heavy furniture to block doors", "Important documents and medications to a waterproof container or higher shelf", "All mirrors" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "During a flash flood, why is entering moving water dangerous?",
                    choices = new[] { "Water can be colder than it looks", "Moving water—even shallow—can sweep a person or vehicle away", "It improves visibility" },
                    correctIndex = 1
                }
            },
            // Hard (mode 1)
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "You’re on the second floor and water begins to rise rapidly outside. What’s the best immediate plan?",
                    choices = new[] {
                        "Go to the roof only if escape routes from the building are blocked and you can signal rescuers",
                        "Attempt to swim through rising water to reach a neighbor",
                        "Break windows to let water flow through the house"
                    },
                    correctIndex = 0
                },
                new QuizQuestion_water {
                    question = "Which action is most important if you must shelter on an upper floor during a flood?",
                    choices = new[] {
                        "Bring as many heavy appliances as you can upstairs",
                        "Take a waterproof bag with water, medkit, radio/phone, and important documents",
                        "Turn off power to keep your lights on longer"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "After floodwaters recede, what should you check first before re-entering a building?",
                    choices = new[] {
                        "That food in the fridge still smells fine",
                        "That the structure is safe (no major foundation damage) and utilities are shut off if advised",
                        "That paint on the walls is not peeling"
                    },
                    correctIndex = 1
                }
            }
        },

        // NPC2 - Evacuation and route planning
        [QuizGroup_water.NPC2] = new[]
        {
            // Normal
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "What should you include in a 'go-bag' for fast evacuation during floods or other disasters?",
                    choices = new[] { "Only cash and sunglasses", "Essentials: water, medkit, flashlight, radio, whistle, copies of ID", "Only a single change of clothes" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "When evacuating by car during storm flooding, which is the safest choice?",
                    choices = new[] { "Drive through standing water if unsure of depth", "Avoid driving through flooded roads; turn around and find an alternate route", "Speed across shallow puddles" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Why is it important to have multiple evacuation routes planned?",
                    choices = new[] { "So you can choose the nicest scenery", "One route may be blocked by debris or high water", "It isn't necessary; one route is enough" },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "You are evacuating on foot during a flood and encounter a fast-flowing stream on your planned route. Best choice?",
                    choices = new[] {
                        "Attempt to cross quickly while holding belongings",
                        "Turn back and find a longer but safer route on higher ground",
                        "Remove shoes and wade through barefoot to avoid slipping"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "If a shelter is provided but you have a pet, what should you do before going there?",
                    choices = new[] {
                        "Leave the pet at home and go to the shelter without informing anyone",
                        "Check shelter pet policy and bring pet supplies or find a pet-friendly shelter/arrangement",
                        "Release the pet outdoors so it can fend for itself"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "When routing during night evacuation in flood conditions, which tool is most critical to keep with you?",
                    choices = new[] {
                        "A deck of playing cards",
                        "A reliable light source (flashlight/headlamp) and a means of communication (radio/phone)",
                        "Extra decorative lights"
                    },
                    correctIndex = 1
                }
            }
        },

        // NPC3 - Personal safety and first aid during disasters
        [QuizGroup_water.NPC3] = new[]
        {
            // Normal
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "If someone is injured during a flood and bleeding heavily, what is a first step you can take with items from a medkit?",
                    choices = new[] { "Apply direct pressure to the wound with clean dressing", "Give them water to drink immediately while still bleeding", "Move them quickly through deep water" },
                    correctIndex = 0
                },
                new QuizQuestion_water {
                    question = "Why is keeping a whistle in your survival kit helpful during a disaster?",
                    choices = new[] { "It helps you call family for dinner", "It allows you to signal rescuers if you are trapped or out of sight", "It scares away helpful volunteers" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "If someone is showing signs of hypothermia after being wet and cold, what should you do first?",
                    choices = new[] { "Give them strong caffeine drinks", "Move them to dry clothing/blankets and get them to a warmer environment", "Force them to exercise vigorously" },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "You find a person with a possible broken limb in a flooded area. Which is the safest immediate action?",
                    choices = new[] {
                        "Attempt to realign the limb yourself before moving them",
                        "Stabilize the limb in the position found with padding and seek professional help as soon as safely possible",
                        "Place the limb directly under water to clean it while moving"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "During a mass evacuation, what is the best way to manage limited medkit supplies?",
                    choices = new[] {
                        "Use expensive items on every minor scratch",
                        "Prioritize life‑threatening needs first (severe bleeding, airway) and triage minor injuries for later care",
                        "Give all supplies to the most vocal person"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "If disinfecting a small wound without sterile saline, which medkit item is typically appropriate to use first?",
                    choices = new[] {
                        "Antiseptic wipes or solution to clean around the wound before dressing",
                        "Drinking water from the nearest source",
                        "Perfume or strong-smelling lotion"
                    },
                    correctIndex = 0
                }
            }
        },

        // NPC4 - Communication and power during disasters
        [QuizGroup_water.NPC4] = new[]
        {
            // Normal
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "Why is a battery-powered or hand-crank radio recommended in emergency kits?",
                    choices = new[] {
                        "To listen to music non-stop",
                        "To receive official alerts and updates when power and cell service may be down",
                        "To call emergency services directly"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "If phone networks are congested during a disaster, what is an effective alternative for getting information?",
                    choices = new[] { "Use social media only", "Listen to emergency broadcasts on a radio and follow official channels", "Keep calling random numbers hoping one connects" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Which power source is most useful for charging small devices when the grid is down?",
                    choices = new[] { "Hand crank or solar charger and spare battery packs", "Standard household power strip", "Gasoline car stereo" },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "Your community issues a boil-water advisory after flooding. What communication step should you take if you rely on community pumps?",
                    choices = new[] {
                        "Assume the pumps are fine and continue using them",
                        "Listen to official radio updates for when water is safe and follow instructions for disinfecting water at home",
                        "Turn on all faucets to flush contaminants away"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Which combination of items is best for maintaining communication and getting emergency info during a prolonged outage?",
                    choices = new[] {
                        "Battery-powered radio, extra batteries, and a charged power bank or solar charger for phones",
                        "Only a single, uncharged smartphone",
                        "A loudspeaker without batteries"
                    },
                    correctIndex = 0
                },
                new QuizQuestion_water {
                    question = "If you must conserve battery power for a radio during a long event, which is a good practice?",
                    choices = new[] {
                        "Leave the radio on continuous at full volume",
                        "Use the radio only for scheduled updates and lower volume/scan less frequently",
                        "Use the radio as a flashlight"
                    },
                    correctIndex = 1
                }
            }
        },

        // NPC5 - General multi-hazard survival
        [QuizGroup_water.NPC5] = new[]
        {
            // Normal
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "Which of the following is a common priority in most disaster situations?",
                    choices = new[] { "Finding entertainment", "Ensuring life‑safety (people), then shelter and supplies", "Collecting souvenirs" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "What is the best long-term strategy for household disaster preparedness?",
                    choices = new[] { "Rely only on neighbors to help", "Maintain an emergency plan, kit, and regular practice/drills", "Only purchase nonperishable snacks" },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Which action reduces secondary hazards after flood damage?",
                    choices = new[] { "Leaving electrical systems powered on in wet areas", "Shutting off electricity and gas if flooding affected utilities and you were advised to do so", "Using candles in all wet rooms" },
                    correctIndex = 1
                }
            },
            // Hard
            new List<QuizQuestion_water>
            {
                new QuizQuestion_water {
                    question = "When rebuilding or returning after a major flood, which professional should you consult first to ensure structural safety?",
                    choices = new[] {
                        "A real estate agent",
                        "A qualified building inspector or structural engineer",
                        "A landscaper"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "Which practice helps communities reduce flood risk over time?",
                    choices = new[] {
                        "Ignoring local floodplain maps",
                        "Advocating for and supporting improved drainage, flood defenses, and responsible land use planning",
                        "Building homes on riverbanks without permits"
                    },
                    correctIndex = 1
                },
                new QuizQuestion_water {
                    question = "If a multi-hazard emergency includes both flooding and chemical spills, what is the safest immediate priority?",
                    choices = new[] {
                        "Focus only on saving belongings",
                        "Avoid contaminated areas, follow official evacuation orders, and inform responders about hazards",
                        "Attempt to neutralize chemical spills yourself"
                    },
                    correctIndex = 1
                }
            }
        },

        // Medkit1..4 - First aid and medkit usage
        [QuizGroup_water.Medkit1] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "What basic medkit item is essential to stop bleeding in an emergency?",
                    choices = new[] {
                        "Gauze pads and adhesive tape to apply direct pressure",
                        "Sunglasses",
                        "A hairbrush"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "When using a tourniquet for severe limb bleeding, what is a correct practice?",
                    choices = new[] {
                        "Apply it over clothing if necessary, tighten until bleeding slows, and note the time of application",
                        "Loosen it every few minutes to improve comfort",
                        "Place it directly over the wound site"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Medkit2] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Which medkit item helps treat minor cuts and reduce infection risk?",
                    choices = new[] {
                        "Antiseptic wipes or solution",
                        "Extra batteries",
                        "Plastic wrap"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "If someone is unconscious but breathing after a disaster injury, what should you do?",
                    choices = new[] {
                        "Place them in the recovery position and monitor breathing while seeking help",
                        "Give them food and water immediately",
                        "Leave them in whatever position you found them"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Medkit3] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "How often should you inspect a medkit to ensure it's ready for emergencies?",
                    choices = new[] {
                        "At least every six months to replace expired or used items",
                        "Only when you remember",
                        "Never—medkits last forever"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "When treating a contaminated wound in the field, what is a reasonable immediate step if sterile supplies are limited?",
                    choices = new[] {
                        "Clean the wound with the cleanest available water, apply antiseptic if available, and dress with a clean covering while seeking professional care",
                        "Leave the wound open and exposed to air",
                        "Cover it with clothing pulled from someone else without cleaning"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Medkit4] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Which medkit item helps manage sprains or strains during evacuation?",
                    choices = new[] {
                        "Elastic (ACE) bandage and a cold pack",
                        "A candle",
                        "A heavy book"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "You must move an injured person a short distance during an evacuation. What is an important consideration?",
                    choices = new[] {
                        "Avoid unnecessary movement if they may have a spinal injury; stabilize and wait for trained help if possible",
                        "Lift quickly without coordinating with helpers",
                        "Drag them by their limbs immediately"
                    },
                    correctIndex = 0
                }
            }
        },

        // Water1..4 - Water storage & purification
        [QuizGroup_water.Water1] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Why is storing bottled water part of emergency preparedness?",
                    choices = new[] {
                        "Because water is essential for drinking, cooking, and hygiene when supplies are disrupted",
                        "Because bottled water tastes better",
                        "Because it decorates the emergency kit"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "What is the recommended short-term water amount to store per person per day?",
                    choices = new[] {
                        "One gallon (about 3.8 liters) per person per day for at least three days",
                        "Half a cup per day",
                        "No specific amount is necessary"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Water2] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "If bottled water runs out, what is a safe household method to make water potable?",
                    choices = new[] {
                        "Boil water for at least one minute (longer at high altitude) to kill pathogens",
                        "Add sugar to it",
                        "Expose it to sunlight briefly and drink immediately"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "When using household bleach to disinfect water, which is a correct practice (if using regular unscented bleach ~5–6% sodium hypochlorite)?",
                    choices = new[] {
                        "Add about 8 drops (approximately 1/8 teaspoon) of bleach per gallon, mix and wait 30 minutes before use (follow product guidance)",
                        "Add bleach until it smells strongly of chlorine and drink immediately",
                        "Always add a full capful per glass"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Water3] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Which sign most strongly suggests untreated water is unsafe to drink?",
                    choices = new[] {
                        "Cloudiness, unusual color, or a bad odor",
                        "It is warm",
                        "It was collected from a municipal tap"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "If you must use a natural water source, what combination of steps gives the best chance of making it safe with limited gear?",
                    choices = new[] {
                        "Filter through cloth, then boil for at least one minute or use proper purification tablets/systems",
                        "Just strain through a T-shirt and drink",
                        "Expose it to daylight for a few minutes"
                    },
                    correctIndex = 0
                }
            }
        },
        [QuizGroup_water.Water4] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Where should you store emergency water to keep it safe?",
                    choices = new[] {
                        "In clean, food-grade containers stored in a cool, dark place",
                        "Directly in sunlight on the roof",
                        "Open buckets in the garage"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "If you find sealed commercial bottled water after a flood, what is a safe practice before drinking?",
                    choices = new[] {
                        "If the seal is intact and bottles were stored properly, they are usually safe; otherwise treat before drinking",
                        "Assume it's contaminated regardless of packaging",
                        "Pour it into open containers for later"
                    },
                    correctIndex = 0
                }
            }
        },

        // Items - Whistle, SafetyHelmet, Radio
        [QuizGroup_water.Whistle1] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "What is the main purpose of carrying a whistle in your emergency kit?",
                    choices = new[] {
                        "To signal rescuers if you are trapped or need to draw attention",
                        "To use as a toy to entertain children",
                        "To scare away rescue animals"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Which whistle signal is widely recognized as a distress call?",
                    choices = new[] {
                        "Three short blasts repeated at regular intervals",
                        "One long continuous blast without pause",
                        "Random short blows"
                    },
                    correctIndex = 0
                }
            }
        },

        [QuizGroup_water.SafetyHelmet] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "Why might a safety helmet be useful during flood or storm cleanup?",
                    choices = new[] {
                        "It protects your head from falling debris, branches, and accidental impacts during cleanup",
                        "It makes you look official",
                        "It is mostly decorative"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "What is a proper helmet practice during multi-day cleanup efforts?",
                    choices = new[] {
                        "Keep it on when in areas with overhead hazards and ensure the chin strap fits snugly",
                        "Only wear it for selfies",
                        "Loosen the straps to make it more comfortable"
                    },
                    correctIndex = 0
                }
            }
        },

        [QuizGroup_water.Radio1] = new[]
        {
            // Normal
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "What advantage does a hand-crank or battery radio offer in an emergency?",
                    choices = new[] {
                        "Access to official updates, weather alerts, and instructions when other communications fail",
                        "Better music selection than streaming services",
                        "Long-distance phone calls"
                    },
                    correctIndex = 0
                }
            },
            // Hard
            new List<QuizQuestion_water> {
                new QuizQuestion_water {
                    question = "If you only have one portable radio and limited batteries during a prolonged event, which listening strategy conserves power while keeping you informed?",
                    choices = new[] {
                        "Listen for scheduled official updates, turn the radio off between updates, and use low volume or earphones",
                        "Keep the radio on continuously at high volume",
                        "Use it only to play music to calm people"
                    },
                    correctIndex = 0
                }
            }
        },
    };

    // Get list for a specific group and mode (0 Normal, 1 Hard)
    public static List<QuizQuestion_water> Get(QuizGroup_water group, int mode)
    {
        mode = Mathf.Clamp(mode, 0, 1);
        return Bank[group][mode];
    }

    // Get list for the current mode from DataManager
    public static List<QuizQuestion_water> Get(QuizGroup_water group)
    {
        int mode = DataManager.Instance != null ? DataManager.Instance.currentMode : 0;
        mode = Mathf.Clamp(mode, 0, 1);
        return Bank[group][mode];
    }

    // Convenience: get one random question (optionally excluding already-used indices you track elsewhere)
    public static QuizQuestion_water GetRandom(QuizGroup_water group, System.Random rng = null)
    {
        var list = Get(group);
        if (list == null || list.Count == 0) return null;
        int i = rng == null ? UnityEngine.Random.Range(0, list.Count) : rng.Next(list.Count);
        return list[i];
    }
}