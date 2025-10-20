using UnityEngine;

[CreateAssetMenu(fileName = "NewFloodFact", menuName = "Flood/Fact")]
public class FloodFact : ScriptableObject
{
    [TextArea(3, 10)]
    public string factTitle;   // A short title for the fact
    [TextArea(5, 20)]
    public string factDescription; // The detailed fact
}
