using UnityEngine;

[System.Serializable]
public class ObjectState
{
    public string id;
    public float[] position;
    public float[] rotation;
    public bool activeSelf;
    public float[] velocity;
    public float[] angularVelocity;
}

[System.Serializable]
public class PlayerState
{
    // Earth trial
    public int hearts;
    public float panic;
    public int medkits;
    public int water;
    public bool isHelmetUsed;
    public bool hasWhistle;
    public bool isCovered;
    
    // Fire trial progression flags (EXISTING - working)
    public bool hasTeleportedToHouseB;
    public bool hasPickedUpExtinguisher;
    public bool hasCompletedExtinguisherQuizzes;
    public bool allFiresOut;
    
    // ✅ NEW: Mr. Kitty state
    public bool mrKittyRescued;
    
    // ✅ NEW: Item pickup state
    public string[] pickedUpItemIDs;
    
    // ✅ NEW: Oxygen state
    public float currentOxygen;
    public bool isTowelEquipped;
    
    // ✅ NEW: Hot door state
    public bool doorOpenedWithTowel;
    public bool touchedHotHandle;
    
    // ✅ NEW: Locked door state
    public bool hasKey;
    public bool doorUnlocked;
    public bool hasTriedDoor;
    public bool timerWasRunning;
    public float savedTime;
    
    // ✅ NEW: Breaker puzzle state
    public bool breakerPuzzleComplete;
    
    // ✅ NEW: Door fire trigger state
    public bool fireMessageShown;
    
    // ✅ NEW: SDR trigger state
    public bool sdrTriggered;
    
    // ✅ NEW: Objective stage
    public int objectiveStage;
    
    // ✅ NEW: Window escape state
    public bool windowTried;
}

[System.Serializable]
public class FlagState
{
    public string id;
    public bool[] behavioursEnabled;
    public bool[] objectsActive;
}

[System.Serializable]
public class WorldSaveData
{
    public int mode;
    public int trialIndex;
    public ObjectState[] objects;
    public PlayerState player;
    public FlagState[] flags;
}