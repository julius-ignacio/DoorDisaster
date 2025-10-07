using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ChangeNpcHelpButtonText : MonoBehaviour
{
    public TMP_Text BtnText;
    public NPC_GiveId nPC_GiveId;
    void Start()
    {
        BtnText.text = "Help";
    }


    void OnTriggerEnter(Collider other)
    {
        switch (nPC_GiveId.NpcId)
        {
            case 9: BtnText.text = "Drink water"; break;
            case 11: BtnText.text = "Drink water"; break;
            case 12: BtnText.text = "Get whistle"; break;
            case 10: BtnText.text = "Drink water"; break;
            case 8: BtnText.text = "Use medkit"; break;
            case 7: BtnText.text = "Use medkit"; break;
            case 6: BtnText.text = "Use medkit"; break;

            default: BtnText.text = "Help"; break;
         }
    }


    void OnTriggerExit()
    {
        BtnText.text = "Help";
    }
}
