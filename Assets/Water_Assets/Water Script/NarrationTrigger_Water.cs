using UnityEngine;

namespace Narrate
{
    /// <summary>
    /// Handles narration when an item is collected — plays subtitle UI text from the NarrationSystem prefab.
    /// </summary>
    public class NarrationTrigger_Water : MonoBehaviour
    {
        [Header("Narrate Asset References")]
        public ProximityNarrationTrigger narrationTrigger; // Reference to the Narrate trigger
        public Transform player; // Optional, assigned automatically

        [Header("Dialog Settings")]
        [TextArea(2, 5)]
        public string customDialog; // The line of text to display
        public bool playInstantly = true; // Whether to skip proximity check and play immediately
        public bool disableAfterPickup = true; // Disable object after triggering

        private bool hasPlayed = false;

        private void Awake()
        {
            // Disable trigger until activated
            if (narrationTrigger != null)
                narrationTrigger.enabled = false;
        }

        /// <summary>
        /// Called when the item is collected or triggered.
        /// </summary>
        public void ActivateNarration()
        {
            if (hasPlayed || narrationTrigger == null)
                return;

            hasPlayed = true;

            // Assign player dynamically if not already
            if (narrationTrigger.triggeredBy == null && player != null)
                narrationTrigger.triggeredBy = player;

            // ✅ Display dialog text on SubtitleManager (UI)
            if (!string.IsNullOrEmpty(customDialog))
            {
                SubtitleManager subtitle = FindObjectOfType<SubtitleManager>();
                if (subtitle != null)
                {
                    subtitle.DisplaySubtitle(customDialog);
                    Debug.Log($"🎙️ Showing subtitle: {customDialog}");
                }
                else
                {
                    Debug.LogError("❌ No SubtitleManager found in the scene. Make sure the NarrationSystem prefab is active.");
                }
            }

            // ✅ Activate the proximity narration if needed
            narrationTrigger.enabled = true;

            // Optionally trigger instantly
            if (playInstantly)
                narrationTrigger.Trigger();

            // Disable object if required
            if (disableAfterPickup)
                gameObject.SetActive(false);
        }

        // ✅ Automatically activate narration when the player collides
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.transform;
                ActivateNarration();
            }
        }
    }
}
