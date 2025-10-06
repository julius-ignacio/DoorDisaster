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
              AudioManager.Instance.PlaySFX(7);
            ButtonText.text = "Turn ON"; lights.SetActive(false);
        }
        else
        {
              AudioManager.Instance.PlaySFX(7);
            lights.SetActive(true); ButtonText.text = "Turn OFF";
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
