using UnityEngine;

public class NpcIcons : MonoBehaviour
{
    public GameObject[] npcIcons;
    public int IconIndex;
    void Start()
    {
        foreach (var icon in npcIcons)
        {
            icon.SetActive(false);
        }

 
    }


    public void makeIconActive()
    {
        IconIndex++;
        npcIcons[IconIndex - 1].SetActive(true);
    }
}
