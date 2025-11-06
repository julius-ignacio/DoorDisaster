using UnityEngine;

public class FootstepSurfaceTrigger : MonoBehaviour
{
    public FootstepSurface surfaceType;

    private void OnTriggerEnter(Collider other)
    {
        var movement = other.GetComponentInParent<Movements>();
        if (movement != null)
        {
            movement.currentSurface = surfaceType;
            Debug.Log($"Player stepped on {surfaceType}");
        }
    }
}
