using UnityEngine;
using System.Collections;

public class FireTrigger : MonoBehaviour
{
    [Header("Fire Settings")]
    public GameObject firePrefab;   // Assign your fire prefab in Inspector
    public Transform spawnPoint;    // Where fire appears

    [Header("Dependencies")]
    public DoorFireTrigger doorFireTrigger; // 👈 Assign in Inspector

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            Debug.Log("Trigger ignored. Either already triggered or wrong tag: " + other.name);
            return;
        }

        // ✅ Only spawn fire if DoorFireTrigger event has happened
        if (doorFireTrigger != null && doorFireTrigger.HasShownFireMessage())
        {
            Debug.Log("Fire triggered by: " + other.name);
            hasTriggered = true;
            StartCoroutine(SpawnFireWithDelay());
        }
        else
        {
            Debug.Log("Fire not spawned - DoorFireTrigger event hasn't happened yet");
        }
    }

    private IEnumerator SpawnFireWithDelay()
    {
        yield return new WaitForSeconds(1f);

        // Spawn fire after 1 second
        if (firePrefab != null && spawnPoint != null)
        {
            Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);
            Debug.Log("🔥 Fire spawned at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("FireTrigger: firePrefab or spawnPoint not assigned!");
        }
    }
}