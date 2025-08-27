using UnityEngine;

public class LockerFall : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {
        rb.isKinematic = true; // Start frozen
    }

    public void Fall()
    {
        rb.isKinematic = false; // Enable physics
        rb.AddTorque(Vector3.forward * 100f); // Push it over
    }
}
