using UnityEngine;

public class FireTrigger : MonoBehaviour
{
    public GameObject firePrefab;   // Assign your fire prefab in Inspector
    public Transform spawnPoint;    // Where fire appears

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("Fire triggered by: " + other.name);

            // Spawn fire with the prefab's own rotation
            Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);

            hasTriggered = true;
        }
        else
        {
            Debug.Log("Trigger ignored. Either already triggered or wrong tag: " + other.name);
        }
    }
}
