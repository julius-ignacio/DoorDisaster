using UnityEngine;

public class ShowBtnTrigger : MonoBehaviour
{
    public GameObject button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button.SetActive(false); // Initially hide the button
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(true);
        }
    }


      private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            button.SetActive(false);
        }
    }
}
