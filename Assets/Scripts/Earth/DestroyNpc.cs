using Unity.VisualScripting;
using UnityEngine;

public class DestroyNpc : MonoBehaviour
{
    public GameObject NpcToDestroy;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == NpcToDestroy) // compare the actual object, not name
        {
            NpcToDestroy.SetActive(false); // instantly disable
        }
    }
}
