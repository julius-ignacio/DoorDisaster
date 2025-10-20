using UnityEngine;
using System.Collections;

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
            hasTriggered = true;
            StartCoroutine(SpawnFireWithDelay());
        }
        else
        {
            Debug.Log("Trigger ignored. Either already triggered or wrong tag: " + other.name);
        }
    }

    private IEnumerator SpawnFireWithDelay()
    {
        yield return new WaitForSeconds(1f);

        // Spawn fire after 1 second
        Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);
    }
}