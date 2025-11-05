using System.Collections;
using UnityEngine;
using System.IO;

[DefaultExecutionOrder(1000)] // run after most Start() methods (extra safety)
public class WorldLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Wait one frame so every other Start has finished.
        yield return null;

        var dm = DataManager.Instance;
        if (dm == null)
        {
            Debug.LogError("WorldLoader: DataManager missing; cannot load world.");
            yield break;
        }

        int trialIndex = dm.currentTrial;
        int mode = dm.currentMode;
        string path = Path.Combine(Application.persistentDataPath, $"save_trial{trialIndex}_mode{mode}.json");

        Debug.Log($"WorldLoader: trial={trialIndex}, mode={mode}, path={path}");

        if (WorldSaveSystem.HasSaveData(trialIndex, mode))
        {
            WorldSaveSystem.LoadWorld(trialIndex, mode);
        }
        else
        {
            Debug.Log("WorldLoader: no save for this trial/mode; starting fresh.");
        }
    }
}