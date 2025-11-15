using UnityEngine;

public class GoToForest : MonoBehaviour
{
    public GameObject TeleportBtn, Player, PlayerTeleportedEntity;

    void Start()
    {
        TeleportBtn.SetActive(false);
        PlayerTeleportedEntity.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportBtn.SetActive(true);
        }
    }

        void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportBtn.SetActive(false);
        }
    }

    public void Teleport()
    {
       Player.SetActive(false);
       PlayerTeleportedEntity.SetActive(true);
       TeleportBtn.SetActive(false);
    }
}
