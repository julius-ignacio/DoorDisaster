using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Audio Sources")]
    public AudioSource audClip;
    public AudioSource audLoop;

    [Header("Clips")]
    public AudioClip[] Clips;


    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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


        public void StopAll()
    {
        foreach (var src in GetComponents<AudioSource>())
            src.Stop();
    }
}
