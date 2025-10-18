    using UnityEngine;

    public class TouchLook : MonoBehaviour
    {
        public float sensitivity = 0.13f;
        private float xRotation = 0f;
        private float yRotation = 0f;
        private int lookFingerId = -1; // track which finger controls camera

        void Update()
        {
    #if UNITY_EDITOR || UNITY_STANDALONE
            // For testing in editor
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * 100f;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * 100f;

            // NORMAL orientation (move mouse right → look right, move up → look up)
            yRotation += mouseX;
            xRotation -= mouseY;
    #else
            // Mobile touch input
            foreach (Touch touch in Input.touches)
            {
                if (lookFingerId == -1 && touch.phase == TouchPhase.Began)
                {
                    lookFingerId = touch.fingerId;
                }

                if (touch.fingerId == lookFingerId)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        // Flip X and Y signs to make movement feel natural
                        Vector2 delta = touch.deltaPosition * sensitivity;
                        yRotation += delta.x;   // swipe right → look right
                        xRotation += delta.y;   // swipe up → look up
                    }

                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                        lookFingerId = -1;
                }
            }
    #endif

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(-xRotation, yRotation, 0f);
        }
    }
