using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnderwaterFogEffect : MonoBehaviour
{
    public Transform player;
    [SerializeField] private Transform water;   // private, but still visible in Inspector
    public Color underwaterFogColor = new Color(0.2f, 0.4f, 0.7f, 0.6f);
    public float underwaterFogDensity = 0.08f;
    public AudioClip underwaterSound;

    private bool isUnderwater = false;
    private Color originalFogColor;
    private float originalFogDensity;
    private bool originalFogEnabled;
    private AudioSource audioSource;

    void Awake()
    {
        // Auto-find any GameObject with the "Underwater" tag if not assigned manually
        if (water == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Underwater");
            if (obj != null)
                water = obj.transform;
            else
                Debug.LogWarning("⚠️ Could not find any GameObject with tag 'Underwater'. Please assign one in the Inspector or tag your water object.");
        }
    }

    void Start()
    {
        // Save current fog settings
        originalFogColor = RenderSettings.fogColor;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogEnabled = RenderSettings.fog;

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        if (underwaterSound != null) audioSource.clip = underwaterSound;
    }

    void Update()
    {
        if (water == null || player == null) return;

        // Check if player's head is below water
        if (player.position.y < water.position.y && !isUnderwater)
        {
            EnterWater();
        }
        else if (player.position.y >= water.position.y && isUnderwater)
        {
            ExitWater();
        }
    }

    void EnterWater()
    {
        isUnderwater = true;

        RenderSettings.fog = true;
        RenderSettings.fogColor = underwaterFogColor;
        RenderSettings.fogDensity = underwaterFogDensity;

        if (audioSource.clip != null) audioSource.Play();
    }

    void ExitWater()
    {
        isUnderwater = false;

        RenderSettings.fog = originalFogEnabled;
        RenderSettings.fogColor = originalFogColor;
        RenderSettings.fogDensity = originalFogDensity;

        if (audioSource.isPlaying) audioSource.Stop();
    }
}
