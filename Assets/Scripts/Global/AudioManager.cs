using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource audClip;
    public AudioSource audLoop;

    [Header("Clips")]
    public AudioClip[] Clips;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlaySFX(0); //victory //walking on grass
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlaySFX(1); //wow //Quake theme
        if (Input.GetKeyDown(KeyCode.Alpha3)) PlaySFX(2); //death //Flood theme
        if (Input.GetKeyDown(KeyCode.Alpha4)) PlaySFX(3); //clap //Fire theme
        if (Input.GetKeyDown(KeyCode.Alpha5)) PlaySFX(4); //hurt
        if (Input.GetKeyDown(KeyCode.Alpha6)) PlaySFX(5); //locker hit
        if (Input.GetKeyDown(KeyCode.Alpha7)) PlaySFX(6); //locker noise
        if (Input.GetKeyDown(KeyCode.Alpha8)) PlaySFX(7); //quake
        if (Input.GetKeyDown(KeyCode.Alpha9)) PlaySFX(8); //heartbeat
        if (Input.GetKeyDown(KeyCode.Alpha0)) PlaySFX(9); //added points sfx
        if (Input.GetKeyDown(KeyCode.Minus)) PlaySFX(10); //foot steps
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index <Clips.Length && Clips[index] != null)
        {
            Debug.Log($"Playing clip: {Clips[index].name}");
            audClip.PlayOneShot(Clips[index]);
        }
    }

        public void PlayLoop(AudioClip clip)
    {
        if (audLoop.clip == clip && audLoop.isPlaying) return; // avoid restarting same loop
        audLoop.clip = clip;
        audLoop.loop = true;
        audLoop.Play();
    }

    public void StopLoop()
    {
        audLoop.Stop();
    }
}
