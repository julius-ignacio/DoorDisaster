using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenLevelSelection : MonoBehaviour
{
    public GameObject Level_selectUI;

    void Start()
    {
        if (Level_selectUI != null) Level_selectUI.SetActive(false);
    }

    public void NormalMode()
    {
        var dm = DataManager.Instance;
        if (dm == null) { Debug.LogError("DataManager is missing in scene."); return; }
        dm.currentMode = 0;
        EnterTrial();
    }

    public void HardMode()
    {
        var dm = DataManager.Instance;
        if (dm == null) { Debug.LogError("DataManager is missing in scene."); return; }
        dm.currentMode = 1;
        EnterTrial();
    }

    void EnterTrial()
    {
        var dm = DataManager.Instance;
        if (dm == null) { Debug.LogError("DataManager is missing in scene."); return; }
        int trial = dm.currentTrial;
        int mode = dm.currentMode;
        bool hasSave = WorldSaveSystem.HasSaveData(trial, mode);
        Debug.Log(hasSave
            ? $"Found existing save for trial {trial}, mode {mode}. Loading scene; WorldLoader will restore."
            : $"No save found for trial {trial}, mode {mode}. Starting new run.");
        // Ensure game is unpaused before switching scenes
        Time.timeScale = 1f;
        AudioListener.pause = false;
        // Optionally hide the level select UI now
        if (Level_selectUI != null) Level_selectUI.SetActive(false);
        SceneManager.LoadScene(GetSceneName(trial));
    }

    private string GetSceneName(int trial)
    {
        switch (trial)
        {
            case 0: return "Fire";
            // case 1: return "Water";
            case 2: return "Earth";
            default:
                Debug.LogWarning($"Unknown trial index {trial}, defaulting to Earth.");
                return "Earth";
        }
    }
}