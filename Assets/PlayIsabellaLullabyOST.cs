using System.Net.NetworkInformation;
using UnityEngine;

public class PlayIsabellaLullabyOST : MonoBehaviour
{
    public AudioManager aud;
    public AudioSource source;
    public GameObject wall;

    void Start()
    {
        wall.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // optional safety check
        {
            AudioClip temp = aud.Clips[10];      // store the first one
            aud.Clips[10] = aud.Clips[13];       // assign slot 13 into slot 10
            aud.Clips[13] = temp;                // put the stored value into slot 13


            aud.PlayLoop(aud.Clips[12]);
            source.volume = 0.5f;
            
            wall.SetActive(true);

        }
    }

}
