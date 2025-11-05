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
    public int hearts;
    public float panic;
    public int medkits;
    public int water;
    public bool isHelmetUsed;

    // NEW
    public bool hasWhistle;
    public bool isCovered;
}

[System.Serializable]
public class WorldSaveData
{
    public int mode;       // 0 Normal / 1 Hard
    public int trialIndex;
    public ObjectState[] objects;
    public PlayerState player;         // added
}