using UnityEngine;

public class UnCoverMechanic : MonoBehaviour
{
    [Tooltip("Reference to the CoverMechanic that manages cover state")]
    public CoverMechanic cover; // assign in Inspector

    void Awake()
    {
        if (cover == null)
            cover = GetComponentInParent<CoverMechanic>() ?? FindObjectOfType<CoverMechanic>(true);
    }

    // Hook this to the UnCover button OnClick
    public void OnButtonClick()
    {
        if (cover != null)
            cover.ApplyCoveredState(false);
        else
            Debug.LogError("[UnCoverMechanic] Missing CoverMechanic reference.");
    }
}