using UnityEngine;

public class slowDownPlayer : MonoBehaviour
{
    public Movements PlayerMovements;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovements.speed = 2f;
        }
    }
}
