using UnityEngine;

public class GiveFactIndex : MonoBehaviour
{
    public discoverFacts _discoverfacts;
    public int indexForFacts;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _discoverfacts.factIndex = indexForFacts;
            _discoverfacts.ReadBtn.SetActive(true);
        }
    }

     void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { _discoverfacts.ReadBtn.SetActive(false); _discoverfacts.facts[_discoverfacts.factIndex].SetActive(false); }
    }
}
