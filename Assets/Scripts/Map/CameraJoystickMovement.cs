
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraJoystickMovement : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3f;

    [Header("Movement Limits")]
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    [Header("Joystick")]
    [SerializeField] private RectTransform joystickArea;
    [SerializeField] private RectTransform joystickHandle;

    private Vector2 inputDirection;
    private Vector2 joystickCenter;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        joystickCenter = joystickArea.rect.center;
    }

    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (cameraTransform == null)
            return;

        Vector3 movement = new Vector3(
            inputDirection.x,
            inputDirection.y,
            0f
        );

        Vector3 newPosition = cameraTransform.position +
            movement * movementSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(
            newPosition.x,
            minPosition.x,
            maxPosition.x
        );

        newPosition.y = Mathf.Clamp(
            newPosition.y,
            minPosition.y,
            maxPosition.y
        );

        cameraTransform.position = newPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickArea,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 direction = localPoint - joystickCenter;

        float radius = joystickArea.rect.width * 0.5f;

        inputDirection = Vector2.ClampMagnitude(
            direction / radius,
            1f
        );

        joystickHandle.anchoredPosition =
            inputDirection * radius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
    }
}
