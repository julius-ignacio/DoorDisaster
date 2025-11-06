using MilkShake;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class PlayerPerformanceCheck : MonoBehaviour
{
    public GameObject warning, barrier, DestroyBtn3, trigger;
    private bool isObjectivesCompleted = false;

    public ConsistentQuake consistentQuake;
    public Shaker shake;
    public GameObject panicmeterUI, quakeIcon, InventoryBtn, hearts, whistlecd, whistleskill, ObjectivesUI, IntroPanelGuide, HUD, PauseBtn;
    public PanicMeterScript panicMeterScript;

    void Start()
    {
        if (barrier) barrier.SetActive(true);
        if (warning) warning.SetActive(true);
        if (DestroyBtn3) DestroyBtn3.SetActive(false);
    }

    void Update()
    {
        if (DataManager.Instance.Npcs_saved >= 3 && DataManager.Instance.factsDiscovered >= 5)
        {
            if (!isObjectivesCompleted)
            {
                isObjectivesCompleted = true;
                if (warning) warning.SetActive(false);
            }

            // Keep HUD visible once objectives are met during this session
            if (HUD) HUD.SetActive(true);
            if (PauseBtn) PauseBtn.SetActive(true);
        }
        else
        {
            isObjectivesCompleted = false;
            if (warning) warning.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Only show the destroy button when objectives are complete
        if (isObjectivesCompleted)
        {
            if (DestroyBtn3) DestroyBtn3.SetActive(true);
            if (warning) warning.SetActive(false);
        }
        else
        {
            if (DestroyBtn3) DestroyBtn3.SetActive(false);
            if (warning) warning.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && DestroyBtn3)
            DestroyBtn3.SetActive(false);
    }

    public void DestroyWall()
    {
        // Guard: only proceed if objectives are complete
        if (!isObjectivesCompleted)
        {
            if (warning) warning.SetActive(true);
            return;
        }

        if (barrier) barrier.SetActive(false);
        AudioManager.Instance.PlaySFX(22);
        if (trigger) trigger.SetActive(false);
        if (DestroyBtn3) DestroyBtn3.SetActive(false);

        // Swap/adjust audio
        AudioClip temp = AudioManager.Instance.Clips[2];
        AudioManager.Instance.Clips[2] = AudioManager.Instance.Clips[1];
        AudioManager.Instance.Clips[1] = temp;
        AudioManager.Instance.PlayLoop(AudioManager.Instance.Clips[20]);
        AudioManager.Instance.audLoop.volume = 0.5f;

        // Stop shake and quakes
        if (shake != null)
        {
            // Reset pose once so disabling doesn’t leave a residual offset
            shake.transform.localPosition = Vector3.zero;
            shake.transform.localEulerAngles = Vector3.zero;
            shake.enabled = false;
        }

        if (consistentQuake != null)
        {
            consistentQuake.enabled = false;
            consistentQuake.PauseQuakes();
        }

        // UI off as desired
        if (panicMeterScript != null)
        {
            panicMeterScript.currHealth = 0;
        }
        if (panicmeterUI) panicmeterUI.SetActive(false);
        if (quakeIcon) quakeIcon.SetActive(false);
        if (InventoryBtn) InventoryBtn.SetActive(false);
        if (hearts) hearts.SetActive(false);
        if (whistlecd) whistlecd.SetActive(false);
        if (ObjectivesUI) ObjectivesUI.SetActive(false);
        if (whistleskill) whistleskill.SetActive(false);

        // Intro guide off; HUD and Pause stay on
        if (IntroPanelGuide) IntroPanelGuide.SetActive(false);
        if (HUD) HUD.SetActive(true);
        if (PauseBtn) PauseBtn.SetActive(true);

        // Persist immediately so the intro stays hidden next time and HUD/Pause are on
        var dm = DataManager.Instance;
        if (dm != null)
        {
            dm.SaveTrialData(dm.currentTrial, dm.currentMode);
            WorldSaveSystem.SaveWorld(dm.currentTrial, dm.currentMode);
        }
    }
}