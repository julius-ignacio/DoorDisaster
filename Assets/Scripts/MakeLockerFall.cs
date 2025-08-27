using UnityEngine;

public class MakeLockerFall : MonoBehaviour
{
    public GameObject[] lockers;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject locker in lockers)
            {
                LockerFall lockerFall = locker.GetComponent<LockerFall>(); // get this locker’s script
                if (lockerFall != null)
                {
                    lockerFall.Fall(); // trigger that locker’s fall
                }
            }

            // // Optional: disable trigger so it only happens once
            // gameObject.SetActive(false);
        }
    }
}
