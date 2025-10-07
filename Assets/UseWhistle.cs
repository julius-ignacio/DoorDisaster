using System.Collections;
using UnityEngine;

public class UseWhistle : MonoBehaviour
{
    [Header("Whistle Settings")]
    public float outlineDuration = 5f;         // How long outlines stay visible
    public KeyCode whistleKey = KeyCode.F;     // Which key activates the whistle

    [Header("Targets")]
    public GameObject[] outlinedObjects;       // Objects to highlight

    private bool isUsingWhistle = false;
    public GameObject ButtonSkill;

    void Update()
    {
        if (Input.GetKeyDown(whistleKey) && !isUsingWhistle)
        {
            StartCoroutine(ShowOutlinesTemporarily());
        }
    }

    public void whistle()
    {
        StartCoroutine(ShowOutlinesTemporarily());
        ButtonSkill.SetActive(false);
    }

    private IEnumerator ShowOutlinesTemporarily()
    {
        isUsingWhistle = true;
        AudioManager.Instance.PlaySFX(21); // index for footsteps in your Clips array


        // Enable silhouette outlines for all objects
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineMode = Outline.Mode.SilhouetteOnly; // show through walls
                outline.OutlineWidth = 8f;                         // make visible
            }
        }

        yield return new WaitForSeconds(outlineDuration);

        // Return outlines to normal (hidden)
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineMode = Outline.Mode.OutlineAll;
            }
        }

        isUsingWhistle = false;
    }
}
