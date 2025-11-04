using UnityEngine;

public class CameraCollision_Water : MonoBehaviour
{
    [Header("Collision Settings")]
    public Transform target;           // 🎯 usually the player head or camera follow pivot
    public float minDistance = 0.3f;   // Minimum distance to target
    public float maxDistance = 3.0f;   // Default camera distance
    public float smooth = 10f;         // Smoothing speed
    public LayerMask collisionMask;    // Layers that block the camera (e.g., Default, Environment)

    private Vector3 dollyDir;
    private float currentDistance;

    void Start()
    {
        dollyDir = transform.localPosition.normalized;
        currentDistance = transform.localPosition.magnitude;
    }

    void LateUpdate()
    {
        // Desired camera position (before collision check)
        Vector3 desiredCameraPos = target.TransformPoint(dollyDir * maxDistance);

        // Raycast from player to camera
        if (Physics.Linecast(target.position, desiredCameraPos, out RaycastHit hit, collisionMask))
        {
            currentDistance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
        }
        else
        {
            currentDistance = maxDistance;
        }

        // Smooth movement of camera to avoid jitter
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            dollyDir * currentDistance,
            Time.deltaTime * smooth
        );
    }
}
