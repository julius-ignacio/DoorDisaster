using UnityEngine;

public class NpcsSaved : MonoBehaviour
{
    [Header("Icons")]
    public GameObject[] npcIcons;
    [Header("Models")]
    public GameObject[] npcModels;
    public int IconIndex;
    void Start()
    {
        foreach (var icon in npcIcons)
        {
            icon.SetActive(false);
        }

        foreach (var npc in npcModels)
        {
            npc.SetActive(false);
        }

 
    }


    public void makeIconActive()
    {
        IconIndex++;
        npcIcons[IconIndex - 1].SetActive(true);
        npcModels[IconIndex - 1].SetActive(true);
    }
}
