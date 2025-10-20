using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick_Water : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private RectTransform bg;       // Background
    private RectTransform handle;   // The moving stick
    private Vector2 inputVector;

    private void Start()
    {
        bg = GetComponent<RectTransform>();
        handle = transform.GetChild(0).GetComponent<RectTransform>(); // first child = handle
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Convert screen point to local point
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bg, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bg.sizeDelta.x);
            pos.y = (pos.y / bg.sizeDelta.y);

            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            // Move handle inside background
            handle.anchoredPosition = new Vector2(
                inputVector.x * (bg.sizeDelta.x / 2),
                inputVector.y * (bg.sizeDelta.y / 2)
            );
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero; // Reset to center
    }

    // Public getters
    public float Horizontal() => inputVector.x;
    public float Vertical() => inputVector.y;
    public Vector2 Direction() => inputVector;
}
