using UnityEngine;

public class TowelPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject towel;
    public SubtitleManager subtitleManager;

    private bool hasPickedUp = false;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasPickedUp)
        {
            if (Input.GetKey(KeyCode.E))
            {
                hasPickedUp = true;

                // Hide towel object (but keep the script GameObject active)
                towel.SetActive(false);

                // Hide objective
                subtitleManager.HideObjective();

                // Slow down oxygen drain
                PlayerOxygen oxygen = other.GetComponent<PlayerOxygen>();
                if (oxygen != null)
                {
                    oxygen.EquipTowel();
                }

                // Messages
                subtitleManager.ShowCustomMessage(
                    "Got the wet towel! This will help me breathe.",
                    2f,
                    () => {
                        subtitleManager.ShowCustomMessage(
                            "Oh no! I need to save the cat!",
                            3f,
                            () => subtitleManager.ShowObjective("Find the cat in the living room")
                        );
                    }
                );
            }
        }
    }

    public bool HasPickedUpTowel()
    {
        return hasPickedUp;
    }
}