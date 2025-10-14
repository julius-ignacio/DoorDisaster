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

 

    public void Whistle()
    {
        if (!isUsingWhistle)
        {
                 StartCoroutine(ShowOutlinesTemporarily());
       ButtonSkill.transform.localScale = new Vector3(-50, -50, -50);

        }
       
    }

    private IEnumerator ShowOutlinesTemporarily()
    {
        isUsingWhistle = true;
        AudioManager.Instance.PlaySFX(21); // plays the whistle sound

        // Enable silhouette outlines for all objects
        foreach (GameObject obj in outlinedObjects)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
                outline.OutlineMode = Outline.Mode.SilhouetteOnly; // show through walls
                outline.OutlineWidth = 8f;
            }
        }

        yield return new WaitForSeconds(outlineDuration);


        // Turn outlines back to normal or hide them
        foreach (GameObject obj in outlinedObjects)
        {

            Outline outline = obj.GetComponent<Outline>();
                              outline.enabled = true;

                outline.OutlineWidth = 3f;
            outline.OutlineMode = Outline.Mode.OutlineAll;
        
        }

        isUsingWhistle = false;

        ButtonSkill.SetActive(false);
    }
}
