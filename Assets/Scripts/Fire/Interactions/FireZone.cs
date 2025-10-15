using UnityEngine;

public class FireZone : MonoBehaviour
{
    public int fireDamage = 5;
    public float damageInterval = 2f;
    private float nextDamageTime;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Movements2 player = other.GetComponent<Movements2>();
            if (player != null && Time.time >= nextDamageTime)
            {
                player.TakeDamage(fireDamage);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("Player damaged! HP now: " + player.currentHealth);
            }
        }
    }
}
