using UnityEngine;
using TMPro;
using Unity.Android.Gradle.Manifest;

public class Objectives_water : MonoBehaviour
{
    public TMP_Text objectivetext;
    public GameObject[] ItemsToCollect;
    public GameObject balconyBarrier;
    public bool radioListened = false;



    public GameNotifier gameNotifier;
    private bool objectivesCompleted = false;


    void Start()
    {
        objectivetext.text = "Listen to the radio.";
    }
    public void UpdateObjectives()
    {
        if (radioListened)
        {
            objectivetext.text = "Turn off the breaker in the basement.";
        }

        if(ItemsToCollect[0].activeSelf == false){
            objectivetext.text = "Collect the water purifier parts.";
        }
    }



   public void ObjectivesComplete()
    {
        if (DataManager.Instance.currentMode == 0)
        {
            // NpcsHelped_Objective.text = "Objective Complete!";
            objectivetext.color = Color.green;
            objectivetext.text = $"Facts discovered ({DataManager.Instance.factsDiscovered}/3)";
        }
        gameNotifier.BarrierRemoved();
    }
}
