using UnityEngine;

public class moveNpcCamera : MonoBehaviour
{
    public Camera NpcCameraShow;
    public Vector3 targetPosition; 
    public Vector3 targetRotation; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Only trigger for Player
        {
  
            NpcCameraShow.enabled = true;
            NpcCameraShow.transform.localPosition = targetPosition;
            NpcCameraShow.transform.localRotation = Quaternion.Euler(targetRotation);


            Debug.Log("NpcCamera moved to target position");
        }
    }

    void Start()
    {
        NpcCameraShow.enabled = false;
    }
}
