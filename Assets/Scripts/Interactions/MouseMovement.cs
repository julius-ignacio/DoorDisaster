using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 2f; // Lowered sensitivity since we removed Time.deltaTime

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        // Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // REMOVED Time.deltaTime to fix shakiness - mouse input is already frame-rate independent
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Control rotation around x axis (Look up and down)
        xRotation -= mouseY;

        // We clamp the rotation so we cant over-rotate (like in real life)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Control rotation around y axis (Look left and right)
        YRotation += mouseX;

        // Applying both rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

        // Handle cursor toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
}