using UnityEngine;
using UnityEngine.UI;

public class SlowDownPlayer : MonoBehaviour
{
    public Movements PlayerMovements;
    public ConsistentQuake consistentQuake;
    public GameObject dizzyEffetctImage;

    void Start()
    {
        dizzyEffetctImage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (consistentQuake.IsQuakeActive)
        {
            PlayerMovements.speed = 1.2f;
            dizzyEffetctImage.SetActive(true);
        }
        else
        {
            PlayerMovements.speed = 3f;
            dizzyEffetctImage.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Keep checking if quake is active WHILE inside the trigger
        PlayerMovements.speed = consistentQuake.IsQuakeActive ? 1.2f : 3f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // When player leaves trigger, always reset speed to default
        PlayerMovements.speed = 3f;
        dizzyEffetctImage.SetActive(false);
    }
}
