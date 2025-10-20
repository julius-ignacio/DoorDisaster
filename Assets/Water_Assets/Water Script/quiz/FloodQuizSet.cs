using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuizSet", menuName = "Flood/Quiz Set")]
public class FloodQuizSet : ScriptableObject
{
    public List<FloodQuestion> questions = new List<FloodQuestion>();
}
