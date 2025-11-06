using System;

[Serializable]
public class WorldSaveEnvelope
{
    public WorldSaveData data; // your existing serializable save
    public long updatedAt;     // unix ms timestamp
    public int version = 1;    // reserve for future migrations
}