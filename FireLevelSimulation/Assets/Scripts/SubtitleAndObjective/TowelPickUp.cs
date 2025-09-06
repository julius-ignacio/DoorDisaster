using UnityEngine;

public class TowelPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject towel;
    public SubtitleManager subtitleManager;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.E))
            {
                // Hide the towel object
                towel.SetActive(false);

                // Hide the objective
                subtitleManager.HideObjective();

                // Show pickup message
                subtitleManager.ShowCustomMessage("Got the wet towel! This will help me breathe.", 2f);
            }
        }
    }
}