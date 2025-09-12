using System;
using UnityEngine;

public class discoverFacts : MonoBehaviour
{
    public AudioManager aud;
    public GameObject ReadBtn;
    public GameObject Trigger;
    public GameObject[] facts;
    public int factIndex;

    void Start()
    {
        ReadBtn.SetActive(false);

        foreach (GameObject fact in facts) fact.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) { ReadBtn.SetActive(true); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) { ReadBtn.SetActive(false); facts[factIndex].SetActive(false); }
    }


    public void ReadFacts()
    {
        Success(factIndex);
    }

    void Success(int index)
    {
        Debug.Log("Player has discovered a fact!");

        facts[index].SetActive(true);
        DataManager.Instance.playerScore_erudition++;
        aud.PlaySFX(9);
        ReadBtn.SetActive(false);
        Trigger.SetActive(false);
    }
}
