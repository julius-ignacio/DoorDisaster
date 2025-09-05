using UnityEngine;

public class disabledCamera : MonoBehaviour
{
    public Camera cam;
    void Start()
    {
        cam.enabled = false; // Disable the camera at the start
    }
}
