using UnityEngine;

public class OpenLevelSelection : MonoBehaviour
{
    public GameObject selectionUi;
    void Start()
    {
        selectionUi.SetActive(false);
    }


    public void OpenSelection()
    {
        selectionUi.SetActive(true);
    }
    

         public void OnTriggerExit()
    {
        selectionUi.SetActive(false);
    }

}
