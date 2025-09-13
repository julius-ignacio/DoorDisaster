using TMPro;
using UnityEngine;

public class RoomLightOnOff : MonoBehaviour
{

    public GameObject lights, ShowBtn;
    public TMP_Text ButtonText;
    void Start()
    {
        ShowBtn.SetActive(false);
        lights.SetActive(false);
    }


    public void OnOff()
    {
        if (lights.activeSelf)
        {
            ButtonText.text = "Turn OFF"; lights.SetActive(false);
        }
        else
        {
            lights.SetActive(true); ButtonText.text = "Turn ON";
        }
    }


    void OnTriggerEnter(Collider other)
    {
        ShowBtn.SetActive(true);
    }

      void OnTriggerExit(Collider other)
    {
        ShowBtn.SetActive(false);
    }
}
