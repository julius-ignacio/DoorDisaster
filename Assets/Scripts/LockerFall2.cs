using UnityEngine;

public class LockerFall2 : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {
        rb.isKinematic = true; // Start frozen
    }

    public void Fall()
    {
        rb.isKinematic = false; // Enable physics
        rb.AddTorque(Vector3.back * 200f); // Push it over
    }
}
