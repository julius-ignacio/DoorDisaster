using UnityEngine;

public class LockerScript : MonoBehaviour
{
    public AudioSource lockersound;
    void Start()
    {
        lockersound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
