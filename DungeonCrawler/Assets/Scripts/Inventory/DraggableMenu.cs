using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableMenu : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    private Vector2 pointerOffset;
    private RectTransform canvasRectTransform;
    private RectTransform panelRectTransform;

    void Awake()
    {
        panelRectTransform = GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Calculate offset between pointer and panel position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, eventData.position, eventData.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (!(Keyboard.current != null && (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)))
            return;


        if (panelRectTransform == null || canvasRectTransform == null)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            Vector2 newPos = localPointerPosition - pointerOffset;

            // Clamp the position to keep menu inside the canvas
            Vector3 minPosition = canvasRectTransform.rect.min - panelRectTransform.rect.min;
            Vector3 maxPosition = canvasRectTransform.rect.max - panelRectTransform.rect.max;

            newPos.x = Mathf.Clamp(newPos.x, minPosition.x, maxPosition.x);
            newPos.y = Mathf.Clamp(newPos.y, minPosition.y, maxPosition.y);

            panelRectTransform.localPosition = newPos;
        }
    }
}
