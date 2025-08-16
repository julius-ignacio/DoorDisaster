using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GoToTrial : MonoBehaviour
{

    public Button actionButton;   // Assign in Inspector
    private string sceneToLoad = "";

    private void Start()
    {
        actionButton.gameObject.SetActive(false); // Hide at start
        actionButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Earth"))
        {
            sceneToLoad = "DoorOfEarth"; // name of your scene
            actionButton.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Water"))
        {
            sceneToLoad = "WaterScene";
            actionButton.gameObject.SetActive(true);
        }
        else if (other.CompareTag("Fire"))
        {
            sceneToLoad = "FireScene";
            actionButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide button when leaving
        if (other.CompareTag("Earth") || other.CompareTag("Water") || other.CompareTag("Fire"))
        {
            sceneToLoad = "";
            actionButton.gameObject.SetActive(false);
        }
    }

    private void OnButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }


}
