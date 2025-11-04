using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Narrate
{
    /// <summary>
    /// Handles narration when triggered — shows subtitle(s) and activates the next objective.
    /// Supports multiple custom dialog lines with individual delay and duration.
    /// Can also be called directly for scripted narration (radio, events, etc.).
    /// </summary>
    [DisallowMultipleComponent]
    public class NarrationTrigger_Water : MonoBehaviour
    {
        [Header("Narrate Asset References")]
        public ProximityNarrationTrigger narrationTrigger;
        public Transform player;

        [System.Serializable]
        public class DialogLine
        {
            [TextArea(2, 5)] public string text;
            [Tooltip("Time before this line appears (seconds).")] public float delay = 0f;
            [Tooltip("How long this line stays visible (seconds).")] public float duration = 4f;
        }

        [Header("Dialog Settings")]
        public List<DialogLine> dialogLines = new List<DialogLine>();
        public bool playInstantly = true;
        public bool disableAfterPickup = true;

        private bool hasPlayed = false;

        private void Awake()
        {
            if (narrationTrigger != null)
                narrationTrigger.enabled = false;
        }

        // 🔹 Trigger-based narration
        public void ActivateNarration(MonoBehaviour runner = null)
        {
            if (hasPlayed) return;
            hasPlayed = true;

            if (narrationTrigger != null && narrationTrigger.triggeredBy == null && player != null)
                narrationTrigger.triggeredBy = player;

            MonoBehaviour safeRunner = runner != null ? runner : FindActiveRunner();
            safeRunner.StartCoroutine(PlayAllDialogLines());
        }

        private IEnumerator PlayAllDialogLines()
        {
            SubtitleManager subtitle = FindObjectOfType<SubtitleManager>();
            if (subtitle == null)
            {
                Debug.LogWarning("⚠️ No SubtitleManager found! Add the NarrationSystem prefab to the scene.");
                yield break;
            }

            foreach (DialogLine line in dialogLines)
            {
                if (string.IsNullOrEmpty(line.text)) continue;

                if (line.delay > 0)
                    yield return new WaitForSeconds(line.delay);

                subtitle.DisplaySubtitle(line.text, line.duration);
                Debug.Log($"🎙️ Subtitle shown: {line.text} (duration: {line.duration}s)");

                yield return new WaitForSeconds(line.duration + 0.25f);
            }

            if (narrationTrigger != null)
                narrationTrigger.Trigger();

            ObjectiveManager_Water objManager = FindObjectOfType<ObjectiveManager_Water>();
            if (objManager != null)
                objManager.TriggerNextObjectiveFromNarration();

            if (disableAfterPickup)
                StartCoroutine(DisableAfterDelay(0.5f));
        }

        private MonoBehaviour FindActiveRunner()
        {
            PlayerInteractor_Water playerRunner = FindObjectOfType<PlayerInteractor_Water>();
            if (playerRunner != null && playerRunner.isActiveAndEnabled)
                return playerRunner;

            if (isActiveAndEnabled)
                return this;

            Debug.LogWarning("⚠️ No active MonoBehaviour found to run coroutine. Narration may not start.");
            return this;
        }

        private IEnumerator DisableAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (disableAfterPickup)
                gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                player = other.transform;
                ActivateNarration(other.GetComponent<MonoBehaviour>());
            }
        }

        // ✅ Direct manual call (e.g. from Radio script or event)
        public void PlayNarration(string lineId, float delay = 0f, float duration = 4f)
        {
            StartCoroutine(PlayNarrationWithTiming(lineId, delay, duration));
        }

        private IEnumerator PlayNarrationWithTiming(string lineId, float delay, float duration)
        {
            if (string.IsNullOrEmpty(lineId)) yield break;

            if (delay > 0)
                yield return new WaitForSeconds(delay);

            SubtitleManager subtitle = FindObjectOfType<SubtitleManager>();
            if (subtitle == null)
            {
                Debug.LogWarning("⚠️ No SubtitleManager found — cannot show narration subtitles.");
                yield break;
            }

            string dialogText = GetLineDialog(lineId);
            if (!string.IsNullOrEmpty(dialogText))
            {
                subtitle.DisplaySubtitle(dialogText, duration);
                Debug.Log($"📻 Playing radio narration: {lineId} (delay: {delay}s, duration: {duration}s)");
                yield return new WaitForSeconds(duration);
            }
            else
            {
                Debug.LogWarning($"⚠️ Unknown narration line ID: {lineId}");
            }
        }

        // 🗒️ Radio broadcast text definitions
        private string GetLineDialog(string lineId)
        {
            switch (lineId)
            {
                case "RadioLine1":
                    return "This is the National Disaster Response Unit. Severe flooding has breached the lower districts. If you are still inside your home, gather essentials and move to higher ground immediately.";
                case "RadioLine2":
                    return "This is not a drill! I repeat, this is not a drill! The flood is moving fast. Gather your things and cut off the power!";
                case "RadioLine3":
                    return "Water levels rising faster than expected... Get to safety! Find what you can and escape!";
                default:
                    return null;
            }
        }
    }
}
