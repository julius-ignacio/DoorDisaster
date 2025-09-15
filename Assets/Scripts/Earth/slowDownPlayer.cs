using UnityEngine;
using UnityEngine.UI;

public class SlowDownPlayer : MonoBehaviour
{
    public Movements PlayerMovements;
    public ConsistentQuake consistentQuake;
    public GameObject SlowIcon;

    void Start()
    {
        SlowIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (consistentQuake.IsQuakeActive)
        {
            PlayerMovements.speed = 1.2f;
            SlowIcon.SetActive(true);
        }
        else
        {
            PlayerMovements.speed = 2f;
            SlowIcon.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Keep checking if quake is active WHILE inside the trigger
        PlayerMovements.speed = consistentQuake.IsQuakeActive ? 1.2f : 2f;
        SlowIcon.SetActive(consistentQuake.IsQuakeActive);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // When player leaves trigger, always reset speed to default
        PlayerMovements.speed = 2f;
        SlowIcon.SetActive(false);
    }
}
