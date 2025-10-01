using TMPro;
using UnityEngine;

public class ChangeNpcHelpButtonText : MonoBehaviour
{
    public TMP_Text BtnText;
    public HeartSys heart;
    void Start()
    {
        BtnText.text = "Help";
    }


    void OnTriggerEnter(Collider other)
    {
        BtnText.text = "Use medkit";
    }


    void OnTriggerExit()
    {
        BtnText.text = "Help";
    }
}
