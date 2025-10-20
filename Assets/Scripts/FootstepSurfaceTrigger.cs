using UnityEngine;

public class FootstepSurfaceTrigger : MonoBehaviour
{
    public FootstepSurface surfaceType;

    private void OnTriggerEnter(Collider other)
    {
        var movement = other.GetComponent<Movements>();
        if (movement != null)
        {
            movement.currentSurface = surfaceType;
        }
    }
}
