using UnityEngine;
using TMPro;

public class Objectives : MonoBehaviour
{
    public TMP_Text NpcsHelped_Objective, FactsDiscovered_Objective;
    public GameNotifier gameNotifier;
 private bool objectivesCompleted = false; 
    public void UpdateObjectives()
    {
        FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
        NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/5";

        if (!objectivesCompleted && DataManager.Instance.Npcs_saved == 5 && DataManager.Instance.factsDiscovered == 3)
        {
            ObjectivesComplete();
            objectivesCompleted = true;
        }
    }


   public void ObjectivesComplete()
    {
        // NpcsHelped_Objective.text = "Objective Complete!";
        NpcsHelped_Objective.color = Color.green;
        FactsDiscovered_Objective.color = Color.green;
        FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
        NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/5)";

        gameNotifier.BarrierRemoved();
    }
}
