[System.Serializable]
public class ObjectState
{
    public string id;
    public float[] position;           // x,y,z
    public float[] rotation;           // x,y,z,w
    public bool activeSelf;            // GameObject active flag
    public float[] velocity;           // optional if Rigidbody exists
    public float[] angularVelocity;    // optional if Rigidbody exists
}

[System.Serializable]
public class PlayerState
{
    //earth
    public int hearts;
    public float panic;
    public int medkits;
    public int water;
    public bool isHelmetUsed;
    public bool hasWhistle;
    public bool isCovered;


    // Fire trial progression flags
    public bool tutorialDone;       // NEW
    public bool wakeUpDone;
    public bool phonePickedUp;
    public bool hotlineCalled;
    public bool backpackPickedUp;   // NEW
    public bool doorClothPickedUp;  // NEW
}


// NEW: behaviour + object active state bundle
[System.Serializable]
public class FlagState
{
    public string id;
    public bool[] behavioursEnabled; // aligns with SavableFlag.behaviours
    public bool[] objectsActive;     // aligns with SavableFlag.objects
}


[System.Serializable]
public class WorldSaveData
{
    public int mode;       // 0 Normal / 1 Hard
    public int trialIndex;
    public ObjectState[] objects;
    public PlayerState player;         // added

        public FlagState[] flags;
}

