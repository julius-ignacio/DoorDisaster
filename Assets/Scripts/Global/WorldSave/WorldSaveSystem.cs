using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class WorldSaveSystem : MonoBehaviour
{
    public static void SaveWorld(int trialIndex, int mode)
    {
        // Include inactive objects so we persist everything that changed
        SavableObject[] allObjects =
            Object.FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        List<ObjectState> states = new List<ObjectState>(allObjects.Length);

        foreach (var so in allObjects)
        {
            var t = so.transform;
            var rb = so.GetComponent<Rigidbody>();

            var st = new ObjectState
            {
                id = so.objectID,
                position = new float[] { t.position.x, t.position.y, t.position.z },
                rotation = new float[] { t.rotation.x, t.rotation.y, t.rotation.z, t.rotation.w },
                activeSelf = so.gameObject.activeSelf
            };

            if (rb != null)
            {
                st.velocity = new float[] { rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z };
                st.angularVelocity = new float[] { rb.angularVelocity.x, rb.angularVelocity.y, rb.angularVelocity.z };
            }

            states.Add(st);
        }

        // Collect player state
        var ps = new PlayerState();

        var hearts = Object.FindObjectsByType<HeartSys>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (hearts.Length > 0)
        {
            ps.hearts = hearts[0].currentHearts;
            ps.isHelmetUsed = hearts[0].isHelmetUsed;
        }

        var panic = Object.FindObjectsByType<PanicMeterScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (panic.Length > 0) ps.panic = panic[0].currHealth;

        var inv = Object.FindObjectsByType<InventoryManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inv.Length > 0)
        {
            ps.medkits = inv[0].medkit;
            ps.water = inv[0].water;
        }

        var whistle = Object.FindObjectsByType<UseWhistle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (whistle.Length > 0 && whistle[0].ButtonSkill != null)
            ps.hasWhistle = whistle[0].ButtonSkill.gameObject.activeSelf;

        var cover = Object.FindObjectsByType<CoverMechanic>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (cover.Length > 0 && cover[0].CoverCamera != null)
            ps.isCovered = cover[0].CoverCamera.enabled;

        // Collect behaviour/object flags (enabled / activeSelf)
        var flagComps = Object.FindObjectsByType<SavableFlag>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        var flagStates = new List<FlagState>(flagComps.Length);
        foreach (var f in flagComps)
        {
            var fs = new FlagState { id = f.id };

            if (f.behaviours != null && f.behaviours.Length > 0)
            {
                fs.behavioursEnabled = new bool[f.behaviours.Length];
                for (int i = 0; i < f.behaviours.Length; i++)
                    fs.behavioursEnabled[i] = f.behaviours[i] != null && f.behaviours[i].enabled;
            }

            if (f.objects != null && f.objects.Length > 0)
            {
                fs.objectsActive = new bool[f.objects.Length];
                for (int i = 0; i < f.objects.Length; i++)
                    fs.objectsActive[i] = f.objects[i] != null && f.objects[i].activeSelf;
            }

            flagStates.Add(fs);
        }

        WorldSaveData saveData = new WorldSaveData
        {
            trialIndex = trialIndex,
            mode = mode,
            objects = states.ToArray(),
            player = ps,
            flags = flagStates.ToArray()
        };

        string path = Path.Combine(Application.persistentDataPath, $"save_trial{trialIndex}_mode{mode}.json");
        File.WriteAllText(path, JsonUtility.ToJson(saveData));
        Debug.Log($"Saved world state to {path}");
    }

    public static bool HasSaveData(int trial, int mode)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save_trial{trial}_mode{mode}.json");
        return File.Exists(path);
    }

    public static void DeleteSave(int trial, int mode)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save_trial{trial}_mode{mode}.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save file: {path}");
        }
    }

    public static void LoadWorld(int trialIndex, int mode)
    {
        string path = Path.Combine(Application.persistentDataPath, $"save_trial{trialIndex}_mode{mode}.json");
        if (!File.Exists(path))
        {
            Debug.Log($"No saved world state found at {path}");
            return;
        }

        string json = File.ReadAllText(path);
        WorldSaveData saveData = JsonUtility.FromJson<WorldSaveData>(json);

        // Include inactive objects so we can restore them too
        SavableObject[] allObjects =
            Object.FindObjectsByType<SavableObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Build a map for fast id lookup
        var map = new Dictionary<string, SavableObject>(allObjects.Length);
        foreach (var so in allObjects)
        {
            if (!string.IsNullOrEmpty(so.objectID) && !map.ContainsKey(so.objectID))
                map.Add(so.objectID, so);
        }

        int applied = 0;
        foreach (var state in saveData.objects)
        {
            if (state == null || string.IsNullOrEmpty(state.id)) continue;
            if (!map.TryGetValue(state.id, out var so)) continue;

            // Apply active first
            so.gameObject.SetActive(state.activeSelf);

            var pos = new Vector3(state.position[0], state.position[1], state.position[2]);
            var rot = new Quaternion(state.rotation[0], state.rotation[1], state.rotation[2], state.rotation[3]);

            var rb = so.GetComponent<Rigidbody>();
            if (rb != null)
            {
                bool prevKin = rb.isKinematic;
                rb.isKinematic = true;
                so.transform.SetPositionAndRotation(pos, rot);
                rb.isKinematic = prevKin;

                if (state.velocity != null && state.velocity.Length == 3)
                    rb.linearVelocity = new Vector3(state.velocity[0], state.velocity[1], state.velocity[2]);
                if (state.angularVelocity != null && state.angularVelocity.Length == 3)
                    rb.angularVelocity = new Vector3(state.angularVelocity[0], state.angularVelocity[1], state.angularVelocity[2]);
            }
            else
            {
                so.transform.SetPositionAndRotation(pos, rot);
            }

            applied++;
        }

        // Restore player state
        var ps = saveData.player;
        if (ps != null)
        {
            var hearts = Object.FindObjectsByType<HeartSys>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (hearts.Length > 0)
                hearts[0].ApplyHelmetUIState(ps.isHelmetUsed, ps.hearts);

            var panic = Object.FindObjectsByType<PanicMeterScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (panic.Length > 0)
                panic[0].currHealth = ps.panic;

            var inv = Object.FindObjectsByType<InventoryManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (inv.Length > 0)
            {
                inv[0].medkit = ps.medkits;
                inv[0].water = ps.water;
            }

            var whistle = Object.FindObjectsByType<UseWhistle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (whistle.Length > 0 && whistle[0].ButtonSkill != null)
            {
                whistle[0].ButtonSkill.gameObject.SetActive(ps.hasWhistle);
                if (whistle[0].cooldownUI != null) whistle[0].cooldownUI.SetActive(false);
            }

            var cover = Object.FindObjectsByType<CoverMechanic>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (cover.Length > 0)
                cover[0].ApplyCoveredState(ps.isCovered);
        }

        // APPLY behaviour/object flags
        if (saveData.flags != null && saveData.flags.Length > 0)
        {
            var flagComps = Object.FindObjectsByType<SavableFlag>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var flagMap = new Dictionary<string, SavableFlag>(flagComps.Length);
            foreach (var f in flagComps)
            {
                if (!string.IsNullOrEmpty(f.id) && !flagMap.ContainsKey(f.id))
                    flagMap.Add(f.id, f);
            }

            foreach (var fs in saveData.flags)
            {
                if (fs == null || string.IsNullOrEmpty(fs.id)) continue;
                if (!flagMap.TryGetValue(fs.id, out var f)) continue;

                if (f.behaviours != null && fs.behavioursEnabled != null)
                {
                    int n = Mathf.Min(f.behaviours.Length, fs.behavioursEnabled.Length);
                    for (int i = 0; i < n; i++)
                    {
                        if (f.behaviours[i] != null)
                            f.behaviours[i].enabled = fs.behavioursEnabled[i];
                    }
                }

                if (f.objects != null && fs.objectsActive != null)
                {
                    int n = Mathf.Min(f.objects.Length, fs.objectsActive.Length);
                    for (int i = 0; i < n; i++)
                    {
                        if (f.objects[i] != null)
                            f.objects[i].SetActive(fs.objectsActive[i]);
                    }
                }
            }
        }

        Debug.Log($"Loaded world state from {path}. Restored {applied}/{saveData.objects.Length} objects and {(saveData.flags?.Length ?? 0)} flag groups.");
    }
}