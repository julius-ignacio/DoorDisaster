using UnityEngine;
using UnityEngine.EventSystems;

public class GameButtonManager_Water : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Joystick Settings")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float handleRange = 100f;
    public bool isMobile = true;

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpPressed;

    private Vector2 inputVector;
    private Vector2 joystickCenter;

    void Start()
    {
        if (joystickBackground == null || joystickHandle == null)
        {
            Debug.LogError("⚠️ Assign both joystickBackground and joystickHandle in the Inspector!");
            return;
        }

        joystickCenter = joystickBackground.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 direction = eventData.position - joystickCenter;
        float distance = Mathf.Min(direction.magnitude, handleRange);
        Vector2 clamped = direction.normalized * distance;

        joystickHandle.position = joystickCenter + clamped;

        // Normalize for movement
        inputVector = clamped / handleRange;
        moveInput = new Vector2(inputVector.x, inputVector.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        moveInput = Vector2.zero;
        joystickHandle.position = joystickCenter;
    }

    // Optional: hook this to a Jump button
    public void OnJumpPressed()
    {
        jumpPressed = true;
    }
}
