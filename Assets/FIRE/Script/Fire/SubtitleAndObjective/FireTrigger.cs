using UnityEngine;
using System.Collections;

public class FireTrigger : MonoBehaviour
{
    [Header("Fire Settings")]
    public GameObject firePrefab;
    public Transform spawnPoint;

    [Header("Dependencies")]
    public DoorFireTrigger doorFireTrigger;

    private bool hasTriggered = false;

    // ✅ Static flag for persistence
    public static bool FireSpawned { get; private set; } = false;
    private GameObject spawnedFire;

    void Start()
    {
        // ✅ Restore fire if it was already spawned
        if (FireSpawned && firePrefab != null && spawnPoint != null)
        {
            spawnedFire = Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);
            hasTriggered = true;
            Debug.Log("🔥 Restored spawned fire at: " + spawnPoint.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        // ✅ Only spawn fire if DoorFireTrigger event has happened
        if (doorFireTrigger != null && doorFireTrigger.HasShownFireMessage())
        {
            Debug.Log("Fire triggered by: " + other.name);
            hasTriggered = true;
            StartCoroutine(SpawnFireWithDelay());
        }
    }

    private IEnumerator SpawnFireWithDelay()
    {
        yield return new WaitForSeconds(1f);

        if (firePrefab != null && spawnPoint != null)
        {
            spawnedFire = Instantiate(firePrefab, spawnPoint.position, firePrefab.transform.rotation);
            FireSpawned = true; // ✅ Set static flag
            Debug.Log("🔥 Fire spawned at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("FireTrigger: firePrefab or spawnPoint not assigned!");
        }
    }

    // ✅ Static methods for save system
    public static void RestoreFireSpawnedState(bool spawned)
    {
        FireSpawned = spawned;
        Debug.Log($"🔥 Restored fire spawned state: {spawned}");
    }

    public static void ResetFireSpawnedProgress()
    {
        FireSpawned = false;
        Debug.Log("🔥 Fire spawned progress reset");
    }
}