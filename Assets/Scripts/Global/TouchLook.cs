    using UnityEngine;

    public class TouchLook : MonoBehaviour
    {
        public float sensitivity = 0.13f;
        private float xRotation = 0f;
        private float yRotation = 0f;
    private int lookFingerId = -1; // track which finger controls camera
        
        private Vector2 smoothInput;
public float smoothSpeed = 10f;

void Update()
{
#if UNITY_EDITOR || UNITY_STANDALONE
    float mouseX = Input.GetAxis("Mouse X") * sensitivity * 100f;
    float mouseY = Input.GetAxis("Mouse Y") * sensitivity * 100f;
    smoothInput = Vector2.Lerp(smoothInput, new Vector2(mouseX, mouseY), Time.deltaTime * smoothSpeed);

    yRotation += smoothInput.x;
    xRotation -= smoothInput.y;
#else
    foreach (Touch touch in Input.touches)
    {
        if (lookFingerId == -1 && touch.phase == TouchPhase.Began)
            lookFingerId = touch.fingerId;

        if (touch.fingerId == lookFingerId && touch.phase == TouchPhase.Moved)
        {
            Vector2 delta = touch.deltaPosition * sensitivity;
            smoothInput = Vector2.Lerp(smoothInput, delta, Time.deltaTime * smoothSpeed);

            yRotation += smoothInput.x;
            xRotation += smoothInput.y;
        }

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            lookFingerId = -1;
    }
#endif

    xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    transform.localRotation = Quaternion.Euler(-xRotation, yRotation, 0f);
}
    }
