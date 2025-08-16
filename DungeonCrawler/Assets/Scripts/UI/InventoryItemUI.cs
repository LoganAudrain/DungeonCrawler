
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TMP_Text itemNameText;
    private ItemStats itemData;
    private InventoryMenu menuRef;
    public InventoryMenu MenuRef => menuRef;
    public Button useButton;
    public Button dropButton;
    public GameObject actionBox;
    public static GameObject currentOpenActionBox;
    private int slotIndex;
    public int SlotIndex => slotIndex;

    private float lastClickTime = 0f;
    private const float doubleClickThreshold = 0.3f; // Seconds

    // Drag-and-drop fields
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Transform originalParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        // Only check if this is the open action box
        if (currentOpenActionBox == actionBox && actionBox.activeSelf)
        {
            // Check for left mouse click
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                float time = Time.unscaledTime;
                bool isDoubleClick = (time - lastClickTime) < doubleClickThreshold;
                lastClickTime = time;

                if (isDoubleClick)
                {
                    actionBox.SetActive(false);
                    currentOpenActionBox = null;
                }

                // Check if pointer is over any UI
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    actionBox.SetActive(false);
                    currentOpenActionBox = null;
                }
                else
                {
                    // If pointer is over UI, check if it's over the action box
                    var pointerData = new PointerEventData(EventSystem.current)
                    {
                        position = UnityEngine.InputSystem.Mouse.current.position.ReadValue()
                    };
                    var results = new System.Collections.Generic.List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, results);

                    bool clickedActionBox = false;
                    foreach (var r in results)
                    {
                        if (r.gameObject == actionBox || r.gameObject.transform.IsChildOf(actionBox.transform))
                        {
                            clickedActionBox = true;
                            break;
                        }
                    }
                    if (!clickedActionBox)
                    {
                        actionBox.SetActive(false);
                        currentOpenActionBox = null;
                    }
                }
            }
        }
    }

    public void Highlight(bool on)
    {
        itemNameText.color = on ? Color.yellow : Color.white;
    }

    public static void HideAllActionBoxes()
    {
        if (currentOpenActionBox != null)
        {
            currentOpenActionBox.SetActive(false);
            currentOpenActionBox = null;
        }
    }

    public void SetData(ItemStats data, int count, InventoryMenu menu, int slotIdx)
    {
        itemData = data;
        menuRef = menu;
        slotIndex = slotIdx;

        if (data.maxStack > 1)
            itemNameText.text = $"{data.itemName} x{count}";
        else
            itemNameText.text = data.itemName;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnUseClicked);

        dropButton.onClick.RemoveAllListeners();
        dropButton.onClick.AddListener(OnDropClicked);

        if (actionBox != null)
            actionBox.SetActive(false); // Hide by default
    }

    public void SetAsEmpty()
    {
        itemNameText.text = "Inventory is empty";
        GetComponent<Button>().interactable = false;
        if (actionBox != null)
            actionBox.SetActive(false);
    }

    private void OnClick()
    {
        if (itemData != null)
        {
            menuRef.SelectItem(itemData);
            ShowActionBoxUnderMouse();
        }
    }

    private void ShowActionBoxUnderMouse()
    {
        if (actionBox == null) return;
        if (currentOpenActionBox != null && currentOpenActionBox != actionBox)
            currentOpenActionBox.SetActive(false);

        currentOpenActionBox = actionBox;

        Canvas rootCanvas = menuRef.GetComponentInParent<Canvas>();
        if (rootCanvas == null)
            rootCanvas = FindFirstObjectByType<Canvas>();
        // Re-parent to root canvas so it's not clipped by the scroll view mask
        actionBox.transform.SetParent(rootCanvas.transform, false);

        // Read mouse position
        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

        // Convert screen position to local point inside root canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            mousePos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out Vector2 localPoint
        );

        localPoint.y -= 5; // Offset a bit below the cursor

        // Position and show
        RectTransform actionRect = actionBox.GetComponent<RectTransform>();
        actionRect.anchoredPosition = localPoint;
        actionBox.SetActive(true);
    }

    private void OnUseClicked()
    {
        menuRef.UseItemAtSlot(slotIndex);
        actionBox.SetActive(false);
    }

    private void OnDropClicked()
    {
        menuRef.DropItemAtSlot(slotIndex);
        actionBox.SetActive(false);
    }

    // --- Drag and Drop Implementation ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
        transform.SetParent(transform.root); // Move to root canvas for proper overlay
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;

        // Check if dropped on another slot
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var r in results)
        {
            var otherSlot = r.gameObject.GetComponent<InventoryItemUI>();
            if (otherSlot != null && otherSlot != this)
            {
                // Swap slots
                menuRef.SwapSlots(this.slotIndex, otherSlot.slotIndex);
                break;
            }
        }
    }
}