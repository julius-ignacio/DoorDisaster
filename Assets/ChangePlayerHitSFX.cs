using UnityEngine;

public class ChangePlayerHitSFX : MonoBehaviour
{

    public AudioManager aud;
    public AudioSource source;


    public int sfxIndex;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {



            AudioClip temp = aud.Clips[5];      // store the first one
            aud.Clips[5] = aud.Clips[sfxIndex];       // assign slot 13 into slot 10
            aud.Clips[sfxIndex] = temp;                // put the stored value into slot 13

        }
    }
}







