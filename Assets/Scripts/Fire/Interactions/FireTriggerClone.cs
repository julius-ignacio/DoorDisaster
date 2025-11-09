using UnityEngine;
using System.Collections;

public class FireTriggerClone : MonoBehaviour
{
    [Header("Fire Settings")]
    public GameObject firePrefab;   // Assign your fire prefab in Inspector
    public Transform spawnPoint;    // Where fire appears

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            Debug.Log("FireTriggerClone ignored. Already triggered or wrong tag: " + other.name);
            return;
        }

        hasTriggered = true;
        Debug.Log("🔥 FireTriggerClone activated by: " + other.name);
        StartCoroutine(SpawnFireWithDelay());
    }

    private IEnumerator SpawnFireWithDelay()
    {
        yield return new WaitForSeconds(1f);

        if (firePrefab != null && spawnPoint != null)
        {
            Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);
            Debug.Log("🔥 FireTriggerClone spawned fire at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("FireTriggerClone: firePrefab or spawnPoint not assigned!");
        }
    }
}
