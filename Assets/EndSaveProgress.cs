using UnityEngine;

public class EndSaveProgress : MonoBehaviour
{
    public GameObject saveBtn;

    void Start()
    {
        saveBtn.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        saveBtn.SetActive(true);
    }

     void save()
    {

    }
}
