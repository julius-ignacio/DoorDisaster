using UnityEngine;
using System.Collections.Generic;

namespace EasyDoorSystem
{
    /// <summary>
    /// Stores keys the player picks up.
    /// Attach this to the Player GameObject.
    /// </summary>
    public class KeyInventory : MonoBehaviour
    {
        // ========== SINGLETON ==========
        public static KeyInventory Instance;

        // ========== LIST OF KEYS PLAYER HAS ==========
        private List<string> keys = new List<string>();

        // ========== SHOW KEYS IN INSPECTOR (for debugging) ==========
        [SerializeField] private List<string> debugKeys = new List<string>();

        // ========== SETUP ==========
        void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            // Keep inspector list updated
            debugKeys = new List<string>(keys);

            // Press K to see your keys in console
            if (Input.GetKeyDown(KeyCode.K))
            {
                Debug.Log("=== MY KEYS ===");
                if (keys.Count == 0)
                {
                    Debug.Log("No keys collected yet!");
                }
                else
                {
                    foreach (string key in keys)
                    {
                        Debug.Log("✓ " + key);
                    }
                }
            }
        }

        // ========== ADD KEY ==========
        public void AddKey(string keyID)
        {
            if (keys.Contains(keyID)) return;
            keys.Add(keyID);
            Debug.Log("Added key: " + keyID);
        }

        // ========== CHECK IF PLAYER HAS KEY ==========
        public bool HasKey(string keyID)
        {
            return keys.Contains(keyID);
        }

        // ========== CLEAR ALL KEYS ==========
        public void ClearKeys()
        {
            keys.Clear();
            Debug.Log("Cleared all keys");
        }
    }
}
