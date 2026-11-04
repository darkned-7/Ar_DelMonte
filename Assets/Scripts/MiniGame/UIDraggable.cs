
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("Destino")]
    [SerializeField] private RectTransform target;

    [SerializeField] private float snapDistance = 100f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Button button;

    private DragDropManager manager;

    private Vector2 pointerOffset;

    private bool isLocked;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        button = GetComponent<Button>();

        // Mantener la pieza disponible para recibir eventos
        canvasGroup.blocksRaycasts = true;
    }

    // El administrador configura cada pieza al iniciar
    public void Configurar(DragDropManager dragManager)
    {
        manager = dragManager;

        BloquearBoton();
    }

    public void BloquearBoton()
    {
        if (button != null)
            button.interactable = false;
    }

    public void DesbloquearBoton()
    {
        if (button != null)
            button.interactable = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked)
            return;

        RectTransform parentRect =
            rectTransform.parent as RectTransform;

        if (parentRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        pointerOffset =
            rectTransform.anchoredPosition - localPointerPosition;

        isDragging = true;

        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked || !isDragging)
            return;

        RectTransform parentRect =
            rectTransform.parent as RectTransform;

        if (parentRect == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        rectTransform.anchoredPosition =
            localPointerPosition + pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked || !isDragging)
            return;

        isDragging = false;

        // IMPORTANTE: permitir nuevamente los raycasts
        canvasGroup.blocksRaycasts = true;

        if (target == null)
            return;

        Vector2 piecePosition =
            RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                rectTransform.position
            );

        Vector2 targetPosition =
            RectTransformUtility.WorldToScreenPoint(
                eventData.pressEventCamera,
                target.position
            );

        float distance = Vector2.Distance(
            piecePosition,
            targetPosition
        );

        if (distance <= snapDistance)
        {
            Encajar();
        }
    }

    private void Encajar()
    {
        rectTransform.position = target.position;

        isLocked = true;

        // La pieza queda bloqueada para el arrastre,
        // pero conserva los raycasts para sus futuros clics.
        canvasGroup.blocksRaycasts = true;

        // Notificar al administrador
        if (manager != null)
        {
            manager.RegistrarPiezaColocada(this);
        }
    }
}
