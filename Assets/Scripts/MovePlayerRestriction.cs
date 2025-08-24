using UnityEngine;

public class MovePlayerRestriction : MonoBehaviour
{
    public GameObject wall;
    public Vector3 targetPosition; 
    public Vector3 targetRotation; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wall.SetActive(true);
            wall.transform.position = targetPosition;
            wall.transform.rotation = Quaternion.Euler(targetRotation);
            Debug.Log("wall moved to target position");
        }
    }
}
