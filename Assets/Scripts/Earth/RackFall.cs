using UnityEngine;

public class RackFall : MonoBehaviour
{
    public Rigidbody rb;
    public Transform fallTarget; // assign the pink plane or an empty GameObject at its direction
    public float torqueForce = 300f;

    void Start()
    {
        rb.isKinematic = true; // Start frozen
    }

    public void Fall()
    {
        rb.isKinematic = false;

        if (fallTarget != null)
        {
            // Get direction towards target
            Vector3 dir = (fallTarget.position - transform.position).normalized;

            // Add torque in that direction
            rb.AddTorque(Vector3.Cross(Vector3.up, dir) * torqueForce);
        }
        else
        {
            // Default fall forward if no target assigned
            rb.AddTorque(Vector3.forward * torqueForce);
        }
    }
}
