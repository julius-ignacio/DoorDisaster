using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HelpNPC : MonoBehaviour
{
    public Movements PlayerMovements;
    public ConsistentQuake consistentQuake;
    public Image npc1, npc2, npc3;

    void Start()
    {
        npc1.enabled = false;
        npc2.enabled = false;
        npc3.enabled = false;
    }


    void OnButtonClick()
    {
        
    }


}
