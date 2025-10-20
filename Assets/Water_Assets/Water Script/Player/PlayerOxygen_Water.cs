using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOxygen_Water : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [SerializeField] private float maxOxygen = 10f;
    [SerializeField] private float drainRate = 1f;
    [SerializeField] private float regenRate = 2f;
    [SerializeField] private Transform water;

    private float currentOxygen;
    public bool isUnderwater = false;

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
    public GameObject damageUI; // ✅ show this when player takes damage

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

        HandleOxygen();
        UpdateUI();
    }

    private void HandleOxygen()
    {
        if (isUnderwater)
            currentOxygen -= drainRate * Time.deltaTime;
        else
            currentOxygen += regenRate * Time.deltaTime;

        currentOxygen = Mathf.Clamp(currentOxygen, 0f, maxOxygen);

        // Drowning damage when oxygen is empty
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

                    // ✅ Show the damage UI
                    if (damageUI != null)
                        StartCoroutine(ShowDamageUI());

                    // 🔊 Play bubble + grunt (optional)
                    if (bubblesSound != null)
                        bubblesSound.Play();

                    if (gruntSound != null)
                    {
                        gruntSound.pitch = Random.Range(0.9f, 1.1f);
                        gruntSound.Play();
                    }

                    // 💀 Check if player died
                    if (heartSys.currentHearts <= 0)
                    {
                        HandleDeath(); // No respawn coroutine now
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

    // ✅ Simple damage UI show coroutine
    private IEnumerator ShowDamageUI()
    {
        damageUI.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        damageUI.SetActive(false);
    }

    // --- Death (No Respawn) ---
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
}
