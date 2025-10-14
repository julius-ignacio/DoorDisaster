using System.Net.NetworkInformation;
using MilkShake;
using UnityEngine;

public class PlayIsabellaLullabyOST_StopQuake : MonoBehaviour
{
    public ConsistentQuake consistentQuake;
    public Shaker shake;
    public GameObject wall;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // optional safety check
        {
            AudioClip temp = AudioManager.Instance.Clips[2];      // store the first one
            AudioManager.Instance.Clips[2] = AudioManager.Instance.Clips[1];       // assign slot 13 into slot 10
            AudioManager.Instance.Clips[1] = temp;                // put the stored value into slot 13


            AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[20]);
            AudioManager.Instance.audLoop.volume = 0.5f;


            shake.enabled = false;
            consistentQuake.enabled = false;
            consistentQuake.PauseQuakes();


            wall.SetActive(true);

        }
    }

}
