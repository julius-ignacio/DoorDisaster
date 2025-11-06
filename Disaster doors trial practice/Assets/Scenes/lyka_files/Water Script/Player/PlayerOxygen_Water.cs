using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOxygen_Water : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 10f;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float regenRate = 2f;
    [SerializeField] private Transform water; // reference to the water surface

    private float currentOxygen;
    private bool isUnderwater = false;

    [Header("Drowning Settings")]
    public float drownInterval = 2f;
    public float minDrownInterval = 0.5f;
    public float accelerationRate = 0.05f;

    private float drownTimer;
    private float timeSinceNoOxygen;

    [Header("References / UI / Audio")]
    private OxygenSys_Water uiOxygen;
    private HeartSysWater heartSys;

    public GameObject gameOverUI;
    public GameObject damageUI;

    public AudioSource deathSound;
    public AudioSource bubblesSound;
    public AudioSource gruntSound;

    private bool isDead = false;

    void Start()
    {
        currentOxygen = maxOxygen;
        uiOxygen = FindObjectOfType<OxygenSys_Water>();
        heartSys = FindObjectOfType<HeartSysWater>();

        drownTimer = drownInterval;
        timeSinceNoOxygen = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (damageUI != null)
            damageUI.SetActive(false);

        if (uiOxygen == null)
            Debug.LogWarning("⚠️ No OxygenSys found in the scene.");
        if (heartSys == null)
            Debug.LogWarning("⚠️ No HeartSysWater found in the scene.");
    }

    void Update()
    {
        if (isDead) return;

        // 🌊 Underwater detection purely based on camera position
        if (water != null)
        {
            Transform cam = Camera.main?.transform;
            if (cam != null)
                isUnderwater = cam.position.y < water.position.y;
        }

        HandleOxygen();
        UpdateUI();
    }

    private void HandleOxygen()
    {
        // 🫧 Oxygen drain or regen
        if (isUnderwater)
            currentOxygen -= drainRate * Time.deltaTime;
        else
            currentOxygen += regenRate * Time.deltaTime;

        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // 💀 Drowning damage when oxygen is empty
        if (currentOxygen <= 0f && isUnderwater)
        {
            timeSinceNoOxygen += Time.deltaTime;
            float currentInterval = Mathf.Max(minDrownInterval, drownInterval - timeSinceNoOxygen * accelerationRate);

            drownTimer -= Time.deltaTime;
            if (drownTimer <= 0f)
            {
                if (heartSys != null)
                {
                    heartSys.UseHeart(1);

                    if (damageUI != null)
                        StartCoroutine(ShowDamageUI());

                    if (bubblesSound != null)
                        bubblesSound.Play();

                    if (gruntSound != null)
                    {
                        gruntSound.pitch = Random.Range(0.9f, 1.1f);
                        gruntSound.Play();
                    }

                    if (heartSys.currentHearts <= 0)
                    {
                        HandleDeath();
                        return;
                    }
                }
                drownTimer = currentInterval;
            }
        }
        else
        {
            drownTimer = drownInterval;
            timeSinceNoOxygen = 0f;
        }
    }

    private void UpdateUI()
    {
        if (uiOxygen != null)
        {
            int oxygenUI = Mathf.RoundToInt((currentOxygen / maxOxygen) * uiOxygen.maxOxygen);
            uiOxygen.currentOxygen = oxygenUI;
            uiOxygen.UpdateOxygen();
        }
    }

    private IEnumerator ShowDamageUI()
    {
        damageUI.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        damageUI.SetActive(false);
    }

    private void HandleDeath()
    {
        isDead = true;
        Debug.Log("💀 Player drowned!");

        if (deathSound != null && !deathSound.isPlaying)
            deathSound.Play();

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public float GetOxygenPercent()
    {
        return currentOxygen / maxOxygen;
    }

    // ✅ Added this method for EndingTrigger_Water
    public void ForceSurface()
    {
        isUnderwater = false;
    }
}
