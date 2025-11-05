using EasyDoorSystem;
using UnityEngine;
using System.Collections;

public class DoorLocked : MonoBehaviour
{
    public GameObject colliderObject, getKeyBtn, getKeyTrigger; 
    public GameObject door, doorlockPrompt;
    public GetKey getkey;

    void Start()
    {
        StartCoroutine(RefreshFromSavedState());
    }

    private IEnumerator RefreshFromSavedState()
    {
        // Wait one frame so WorldLoader has applied activeSelf to the Key object
        yield return null;

        bool keyCollected = false;
        if (getkey != null && getkey.Key != null)
            keyCollected = !getkey.Key.activeSelf; // inactive key means it was collected

        if (keyCollected)
            UnlockNow();
        else
            LockNow();
    }

    private void UnlockNow()
    {
        if (colliderObject) colliderObject.SetActive(false);
        if (doorlockPrompt) doorlockPrompt.SetActive(false);

        var d = door ? door.GetComponent<EasyDoor>() : null;
        if (d) d.enabled = true;
    }

    private void LockNow()
    {
        if (colliderObject) colliderObject.SetActive(true);
        if (doorlockPrompt) doorlockPrompt.SetActive(true);

        var d = door ? door.GetComponent<EasyDoor>() : null;
        if (d) d.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        bool keyCollected = (getkey != null && getkey.Key != null && !getkey.Key.activeSelf);
        if (keyCollected) UnlockNow(); else LockNow();
    }
}