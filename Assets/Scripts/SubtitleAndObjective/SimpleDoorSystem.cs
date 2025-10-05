using System.Collections;
using UnityEngine;

public class SimpleDoorSystem : MonoBehaviour
{
    [Header("References")]
    public SubtitleManager subtitleManager;

    [Header("Settings")]
    public string doorSubtitle = "I should check if this door is safe to open...";
    public float subtitleDuration = 3f;
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private bool subtitlePlayed = false;
    private bool canOpenDoor = false;
    private bool isOpening = false;
    private Quaternion targetRotation;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!subtitlePlayed)
            {
                subtitlePlayed = true;
                StartCoroutine(PlaySubtitleThenUnlock());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (canOpenDoor && !isOpening)
                {
                    OpenDoor();
                }
                else if (!canOpenDoor)
                {
                    subtitleManager.ShowCustomMessage("Wait... let me think about this first.", 2f);
                }
            }
        }
    }

    IEnumerator PlaySubtitleThenUnlock()
    {
        subtitleManager.ShowCustomMessage(doorSubtitle, subtitleDuration);
        yield return new WaitForSeconds(subtitleDuration + 0.5f);
        canOpenDoor = true;
        subtitleManager.ShowCustomMessage("The door seems safe to open now.", 2f);
    }

    void OpenDoor()
    {
        isOpening = true;
        targetRotation = Quaternion.Euler(0f, openAngle, 0f) * transform.rotation;
        subtitleManager.ShowCustomMessage("Door opened!", 1f);
    }

    void Update()
    {
        if (isOpening)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);

            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                transform.rotation = targetRotation;
                isOpening = false;
                this.enabled = false;
            }
        }
    }
}
