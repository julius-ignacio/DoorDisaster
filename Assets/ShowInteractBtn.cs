using UnityEngine;

public class ShowInteractBtn : MonoBehaviour
{

    public GameObject interactBtn;
    public string objectName;
    public ObjectBehaviorEvent objectBehaviorEvent;
    void Start()
    {
        interactBtn.SetActive(false);
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactBtn.SetActive(true);
            objectBehaviorEvent.itemName = objectName;
        }
    }



    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactBtn.SetActive(false);
            objectBehaviorEvent.itemName = " ";
        }
    }
}
