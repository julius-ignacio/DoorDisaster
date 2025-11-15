using UnityEngine;

public class TurnOnRadio : MonoBehaviour
{
    public GameObject radioButton, radio, news;
    public Objectives_water objectivesWater;
    void Start()
    {
        radioButton.SetActive(false);
        news.SetActive(false);
    }


    public void radioTurnedOn()
    {
        if (objectivesWater != null)
        {
            objectivesWater.radioListened = true;
            var outline = radio.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;

            AudioManager.Instance.PlaySFX(36);
            news.SetActive(true);

        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            radioButton.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            radioButton.SetActive(false);
        }
    }



}
