using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GoToTrial : MonoBehaviour
{

    public Button actionButton1, actionButton2;   // Assign in Inspector
    public GameObject actionButtons;
    public string[] sceneToLoad;


    public int trialIndex;   // 0 = Door1, 1 = Door2, 2 = Door3
    private void Start()
    {
        actionButtons.SetActive(false);
        actionButton1.onClick.AddListener(OnButtonClickedStage1);
        actionButton2.onClick.AddListener(OnButtonClickedStage2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Earth"))
        {

            sceneToLoad[0] = "DoorOfEarth1"; // name of your scene
            sceneToLoad[1] = "DoorOfEarth2"; // name of your scene
            trialIndex = 0;
            actionButtons.SetActive(true);

            Debug.Log($"Selected Trial {trialIndex}, Stage {DataManager.Instance.currentStage}");
        }
        else if (other.CompareTag("Water"))
        {
            sceneToLoad[0] = "DoorOfEarth1"; // name of your scene
            sceneToLoad[1] = "DoorOfEarth2"; // name of your scene
            trialIndex = 1;

            actionButtons.SetActive(true);
        }
        else if (other.CompareTag("Fire"))
        {
            sceneToLoad[0] = "DoorOfEarth1"; // name of your scene
            sceneToLoad[1] = "DoorOfEarth2"; // name of your scene
            trialIndex = 2;

            actionButtons.SetActive(true);
        }
    }

     void OnTriggerExit(Collider other)
    {
        // Hide button when leaving
        if (other.CompareTag("Earth") || other.CompareTag("Water") || other.CompareTag("Fire"))
        {
            actionButtons.SetActive(false);
            sceneToLoad[0] = "";
            sceneToLoad[1] = "";
        }
    }

    public void OnButtonClickedStage1()
    {
        if (!string.IsNullOrEmpty(sceneToLoad[0]))
        {
            // Save selection globally
            DataManager.Instance.currentStage = 0;
            DataManager.Instance.currentTrial = trialIndex;

            Debug.Log($"Selected Trial {trialIndex}, Stage {DataManager.Instance.currentStage}");

            SceneManager.LoadScene(sceneToLoad[0]);
        }
    }

    public void OnButtonClickedStage2()
    {
        if (!string.IsNullOrEmpty(sceneToLoad[1]))
        {
            // Save selection globally
            DataManager.Instance.currentStage = 1;
            DataManager.Instance.currentTrial = trialIndex;

            Debug.Log($"Selected Trial {trialIndex}, Stage {DataManager.Instance.currentStage}");
            SceneManager.LoadScene(sceneToLoad[1]);
        }
    }


}