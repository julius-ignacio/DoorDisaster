using TMPro;
using UnityEngine;

public class RoomLightOnOff : MonoBehaviour
{
    public AudioManager aud;
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
            aud.PlaySFX(11);
            ButtonText.text = "Turn ON"; lights.SetActive(false);
        }
        else
        {
            aud.PlaySFX(11);
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
