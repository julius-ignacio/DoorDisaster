using UnityEngine;

public class CeilingFall : MonoBehaviour
{
    public Rigidbody rb;

    void Start()
    {
        // Keep ceiling frozen in place until triggered
        rb.isKinematic = true;
    }

    public void Fall()
    {
        // Enable physics so it drops
        rb.isKinematic = false;

        // Optionally, you can make it fall straight down without rotation
        // but if you want some shake/torque like debris falling, add torque:
        rb.AddTorque(Vector3.right * 150f); 
    }
}
