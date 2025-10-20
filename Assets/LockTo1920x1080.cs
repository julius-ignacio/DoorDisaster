using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LockTo1920x1080 : MonoBehaviour
{
    private void Start()
    {
        const float targetAspect = 1920f / 1080f;
        Camera cam = GetComponent<Camera>();
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Add black bars top and bottom (letterbox)
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Add black bars on sides (pillarbox)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

    // Keep ratio even when orientation changes or device resizes
    private void OnPreCull()
    {
        GL.Clear(true, true, Color.black);
    }
}
