using UnityEngine;

public class PlaneTriggerMoveModel : MonoBehaviour
{
    public GameObject model; // Assign in Inspector
    public Vector3 targetPosition; 
    public Vector3 targetRotation; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Only trigger for Player
        {
            model.transform.position = targetPosition;
    
            model.transform.rotation = Quaternion.Euler(targetRotation);
            Debug.Log("model moved to target position");
        }
    }
}
