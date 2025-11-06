using UnityEngine;
using TMPro;
using Unity.Android.Gradle.Manifest;

public class Objectives : MonoBehaviour
{
    public TMP_Text NpcsHelped_Objective, FactsDiscovered_Objective;
    public GameNotifier gameNotifier;
    private bool objectivesCompleted = false;


    void Start()
    {
                if (DataManager.Instance.currentMode == 0)
        {
            FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
            NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/2)";
        }

        else if(DataManager.Instance.currentMode == 1)
        {
            FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/5)";
            NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/5)";
        }
    }
    public void UpdateObjectives()
    {
        if (DataManager.Instance.currentMode == 0) //easy mode
        {
            Normal();
        }
        else if (DataManager.Instance.currentMode == 1) //hard mode
        {
            Hard();
        }
    }


    void Normal()
    {
            FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
            NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/2)";

            if (!objectivesCompleted && DataManager.Instance.Npcs_saved >= 2 && DataManager.Instance.factsDiscovered >= 3)
            {
                ObjectivesComplete();
                objectivesCompleted = true;
            }
    }
    

    void Hard()
    {
        FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/5)";
        NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/5)";

        if (!objectivesCompleted && DataManager.Instance.Npcs_saved >= 5 && DataManager.Instance.factsDiscovered >= 5)
        {
            ObjectivesComplete();
            objectivesCompleted = true;
        }
    }


   public void ObjectivesComplete()
    {
        if (DataManager.Instance.currentMode == 0)
        {
            // NpcsHelped_Objective.text = "Objective Complete!";
            NpcsHelped_Objective.color = Color.green;
            FactsDiscovered_Objective.color = Color.green;
            FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
            NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/2)";
        }

        else if(DataManager.Instance.currentMode == 1)
        {
            // NpcsHelped_Objective.text = "Objective Complete!";
            NpcsHelped_Objective.color = Color.green;
            FactsDiscovered_Objective.color = Color.green;
            FactsDiscovered_Objective.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/5)";
            NpcsHelped_Objective.text = $"NPCs helped ({DataManager.Instance.Npcs_saved}/5)";
        }

        gameNotifier.BarrierRemoved();
    }
}
