
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[DisallowMultipleComponent]
public class ObjectManipulator : MonoBehaviour
{
    [Header("Objeto")]
    [SerializeField] private Transform target;

    [Header("Cámara")]
    [SerializeField] private Camera interactionCamera;
    [SerializeField] private Transform cameraResetTarget;
    private Vector3 initialCameraLocalPosition;

    [Header("Zona de interaccion")]
    [SerializeField] private RectTransform interactionZone;
    [SerializeField] private Canvas interactionCanvas;
    [SerializeField] private bool ignorePointersOverUI = true;

    [Header("Rotacion")]
    [Tooltip("Desactivado: rotacion local original. Activado: rotacion respecto a los ejes de la camara.")]
    [SerializeField] private bool useCameraRelativeRotation = false;

    [SerializeField] private float rotationSpeed = 0.25f;
    [SerializeField] private bool invertHorizontal;
    [SerializeField] private bool invertVertical;
    [SerializeField] private bool clampRotationX = true;
    [SerializeField] private float minRotationX = -80f;
    [SerializeField] private float maxRotationX = 80f;

    [Header("Escala")]
    [SerializeField] private float minScaleMultiplier = 0.5f;
    [SerializeField] private float maxScaleMultiplier = 2f;
    [SerializeField] private float pinchSensitivity = 1f;
    [SerializeField] private float mouseWheelSensitivity = 0.001f;
    [SerializeField] private Slider scaleSlider;

    // Referencias iniciales
    private Quaternion initialLocalRotation;
    private Vector3 initialLocalScale;

    // Valores acumulados de rotacion
    private float rotationX;
    private float rotationY;

    // Escala actual
    private float scaleMultiplier = 1f;

    // Estado de interacción
    private bool isDragging;
    private bool isPinching;
    private bool pinchAllowed;
    private bool isMouseDragging;

    private float lastPinchDistance;

    private Vector2 lastPointerPosition;
    private Vector2 lastMousePosition;

    // -----------------------------------------
    // INICIALIZACIÓN
    // -----------------------------------------

    private void Awake()
    {
        if (target == null)
            target = transform;

        if (interactionCamera == null)
            interactionCamera = Camera.main;

        if (interactionCanvas == null && interactionZone != null)
            interactionCanvas = interactionZone.GetComponentInParent<Canvas>();

        // Guardar estado inicial del objeto
        initialLocalRotation = target.localRotation;
        initialLocalScale = target.localScale;

        // Asignar la cámara como referencia si no se especificó otra
        if (cameraResetTarget == null && interactionCamera != null)
            cameraResetTarget = interactionCamera.transform;

        // Guardar posición inicial de la cámara
        if (cameraResetTarget != null)
            initialCameraLocalPosition = cameraResetTarget.localPosition;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();

        if (scaleSlider != null)
        {
            scaleSlider.minValue = 0f;
            scaleSlider.maxValue = 1f;

            scaleSlider.onValueChanged.AddListener(SetScaleFromSlider);

            scaleSlider.SetValueWithoutNotify(
                ScaleToSliderValue(1f)
            );
        }
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();

        if (scaleSlider != null)
            scaleSlider.onValueChanged.RemoveListener(SetScaleFromSlider);

        isDragging = false;
        isPinching = false;
        isMouseDragging = false;
    }

    private void OnValidate()
    {
        minScaleMultiplier = Mathf.Max(0.01f, minScaleMultiplier);

        maxScaleMultiplier = Mathf.Max(
            minScaleMultiplier + 0.01f,
            maxScaleMultiplier
        );

        rotationSpeed = Mathf.Max(0.01f, rotationSpeed);
        pinchSensitivity = Mathf.Max(0.01f, pinchSensitivity);
        mouseWheelSensitivity = Mathf.Max(0.0001f, mouseWheelSensitivity);

        if (maxRotationX < minRotationX)
            maxRotationX = minRotationX;
    }

    // -----------------------------------------
    // ACTUALIZACIÓN
    // -----------------------------------------

    private void Update()
    {
        // Prioridad a la interacción táctil.
        if (ProcessTouch())
            return;

        // Mouse para pruebas en PC.
        ProcessMouse();
    }

    // -----------------------------------------
    // ROTACIÓN
    // -----------------------------------------

    private void RotateBy(Vector2 delta)
    {
        if (target == null)
            return;

        Camera cam = interactionCamera != null
            ? interactionCamera
            : Camera.main;

        if (useCameraRelativeRotation && cam == null)
            return;

        float horizontalDirection = invertHorizontal ? 1f : -1f;
        float verticalDirection = invertVertical ? -1f : 1f;

        // Compensar el sentido de rotación en modo relativo a cámara.
        if (useCameraRelativeRotation)
            verticalDirection *= -1f;

        float horizontalDelta =
            delta.x * rotationSpeed * horizontalDirection;

        float verticalDelta =
            delta.y * rotationSpeed * verticalDirection;

        // Aplicar límites de rotación vertical.
        if (clampRotationX)
        {
            float nextRotationX = Mathf.Clamp(
                rotationX + verticalDelta,
                minRotationX,
                maxRotationX
            );

            verticalDelta = nextRotationX - rotationX;
        }

        rotationX += verticalDelta;
        rotationY += horizontalDelta;

        // MODO 1: ROTACIÓN LOCAL ORIGINAL
        if (!useCameraRelativeRotation)
        {
            target.localRotation =
                initialLocalRotation *
                Quaternion.Euler(rotationX, rotationY, 0f);

            return;
        }

        // MODO 2: ROTACIÓN RELATIVA A LA CÁMARA

        Vector3 cameraRight = cam.transform.right;
        Vector3 cameraUp = cam.transform.up;

        Quaternion verticalRotation =
            Quaternion.AngleAxis(verticalDelta, cameraRight);

        Quaternion horizontalRotation =
            Quaternion.AngleAxis(horizontalDelta, cameraUp);

        // Aplicar sobre la rotación mundial actual.
        // Unity considera automáticamente la transformación de MapRoot.
        target.rotation =
            horizontalRotation *
            verticalRotation *
            target.rotation;
    }

    // -----------------------------------------
    // ESCALA
    // -----------------------------------------

    private void ApplyScale(float multiplier)
    {
        if (target == null)
            return;

        scaleMultiplier = Mathf.Clamp(
            multiplier,
            minScaleMultiplier,
            maxScaleMultiplier
        );

        target.localScale = initialLocalScale * scaleMultiplier;

        if (scaleSlider != null)
        {
            scaleSlider.SetValueWithoutNotify(
                ScaleToSliderValue(scaleMultiplier)
            );
        }
    }

    private void ApplyPinchDistance(float currentDistance)
    {
        if (currentDistance <= 0.01f)
            return;

        if (pinchAllowed && lastPinchDistance > 0.01f)
        {
            float ratio = currentDistance / lastPinchDistance;

            ApplyScale(
                scaleMultiplier *
                Mathf.Pow(ratio, pinchSensitivity)
            );
        }

        lastPinchDistance = currentDistance;
    }

    public void SetScaleFromSlider(float normalizedValue)
    {
        float minLog = Mathf.Log(minScaleMultiplier);
        float maxLog = Mathf.Log(maxScaleMultiplier);

        float multiplier = Mathf.Exp(
            Mathf.Lerp(
                minLog,
                maxLog,
                Mathf.Clamp01(normalizedValue)
            )
        );

        ApplyScale(multiplier);
    }

    private float ScaleToSliderValue(float multiplier)
    {
        float minLog = Mathf.Log(minScaleMultiplier);
        float maxLog = Mathf.Log(maxScaleMultiplier);

        float valueLog = Mathf.Log(
            Mathf.Clamp(
                multiplier,
                minScaleMultiplier,
                maxScaleMultiplier
            )
        );

        return Mathf.InverseLerp(minLog, maxLog, valueLog);
    }

    // -----------------------------------------
    // TOUCH ANDROID
    // -----------------------------------------

    private bool ProcessTouch()
    {
        var touches = Touch.activeTouches;

        int count = touches.Count;

        // DOS DEDOS: ZOOM POR PINZA
        if (count >= 2)
        {
            Vector2 a = touches[0].screenPosition;
            Vector2 b = touches[1].screenPosition;

            float distance = Vector2.Distance(a, b);

            isDragging = false;

            if (!isPinching)
            {
                isPinching = true;
                lastPinchDistance = distance;

                pinchAllowed =
                    CanStartGesture(a) &&
                    CanStartGesture(b);
            }
            else
            {
                ApplyPinchDistance(distance);
            }

            return true;
        }

        // UN DEDO: ROTACIÓN
        if (count == 1)
        {
            var touch = touches[0];

            Vector2 position = touch.screenPosition;

            if (isPinching)
            {
                isPinching = false;
                isDragging = false;

                lastPointerPosition = position;

                return true;
            }

            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                isDragging = CanStartGesture(position);
                lastPointerPosition = position;
            }
            else if (
                touch.phase == UnityEngine.InputSystem.TouchPhase.Moved &&
                isDragging
            )
            {
                RotateBy(position - lastPointerPosition);

                lastPointerPosition = position;
            }
            else if (
                touch.phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled
            )
            {
                isDragging = false;
            }

            return true;
        }

        isDragging = false;
        isPinching = false;

        return false;
    }

    // -----------------------------------------
    // MOUSE PC
    // -----------------------------------------

    private void ProcessMouse()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
            return;

        Vector2 position = mouse.position.ReadValue();

        // BOTÓN IZQUIERDO: ROTACIÓN
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isMouseDragging = CanStartGesture(position);
            lastMousePosition = position;
        }
        else if (
            mouse.leftButton.isPressed &&
            isMouseDragging
        )
        {
            RotateBy(position - lastMousePosition);

            lastMousePosition = position;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
            isMouseDragging = false;

        // RUEDA: ZOOM
        float scroll = mouse.scroll.ReadValue().y;

        if (Mathf.Abs(scroll) > 0.01f && CanStartGesture(position))
        {
            ApplyScale(
                scaleMultiplier *
                Mathf.Exp(scroll * mouseWheelSensitivity)
            );
        }
    }

    // -----------------------------------------
    // ZONA DE INTERACCIÓN
    // -----------------------------------------

    private Camera GetCanvasCamera()
    {
        if (interactionCanvas == null && interactionZone != null)
            interactionCanvas = interactionZone.GetComponentInParent<Canvas>();

        if (
            interactionCanvas == null ||
            interactionCanvas.renderMode == RenderMode.ScreenSpaceOverlay
        )
            return null;

        if (interactionCanvas.worldCamera != null)
            return interactionCanvas.worldCamera;

        return interactionCamera;
    }

    private bool IsInsideInteractionZone(Vector2 position)
    {
        if (interactionZone == null)
            return true;

        return RectTransformUtility.RectangleContainsScreenPoint(
            interactionZone,
            position,
            GetCanvasCamera()
        );
    }

    private bool IsOverInteractiveUI(Vector2 position)
    {
        if (!ignorePointersOverUI || EventSystem.current == null)
            return false;

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = position
        };

        var results = new System.Collections.Generic.List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (
                result.gameObject.GetComponentInParent<Selectable>() != null
            )
                return true;
        }

        return false;
    }

    private bool CanStartGesture(Vector2 position)
    {
        return IsInsideInteractionZone(position) &&
               !IsOverInteractiveUI(position);
    }

        // -----------------------------------------
    // CAMBIAR TARGET
    // -----------------------------------------

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogWarning(
                "ObjectManipulator: El nuevo target es null."
            );

            return;
        }

        // Evitar operaciones innecesarias si es el mismo objeto
        if (target == newTarget)
        {
            target.gameObject.SetActive(true);
            return;
        }

        // 1. Desactivar el target anterior
        if (target != null)
        {
            target.gameObject.SetActive(false);
        }

        // 2. Asignar el nuevo target
        target = newTarget;

        // 3. Activar el nuevo target
        target.gameObject.SetActive(true);

        // 4. Guardar su estado inicial
        initialLocalRotation = target.localRotation;
        initialLocalScale = target.localScale;

        // 5. Reiniciar valores de manipulación
        rotationX = 0f;
        rotationY = 0f;
        scaleMultiplier = 1f;

        // 6. Actualizar el slider
        if (scaleSlider != null)
        {
            scaleSlider.SetValueWithoutNotify(
                ScaleToSliderValue(1f)
            );
        }

        // 7. Reiniciar la interacción
        isDragging = false;
        isPinching = false;
        isMouseDragging = false;
    }

    // -----------------------------------------
    // RESTABLECER OBJETO
    // -----------------------------------------

    public void ResetObject()
    {
        if (target == null)
            return;

        // Reiniciar valores de manipulación
        rotationX = 0f;
        rotationY = 0f;
        scaleMultiplier = 1f;

        // Restaurar objeto
        target.localRotation = initialLocalRotation;
        target.localScale = initialLocalScale;

        // Restaurar posición inicial de la cámara
        if (cameraResetTarget != null)
        {
            cameraResetTarget.localPosition = initialCameraLocalPosition;
        }

        // Actualizar slider
        if (scaleSlider != null)
        {
            scaleSlider.SetValueWithoutNotify(
                ScaleToSliderValue(1f)
            );
        }

        // Reiniciar estados de interacción
        isDragging = false;
        isPinching = false;
        isMouseDragging = false;
    }
}
