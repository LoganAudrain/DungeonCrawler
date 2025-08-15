using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    private ItemStats itemData;
    private InventoryMenu menuRef;
    public Button useButton;
    public Button dropButton;
    public GameObject actionBox;
    public static GameObject currentOpenActionBox;

   
    void Update()
    {
        // Only check if this is the open action box
        if (currentOpenActionBox == actionBox && actionBox.activeSelf)
        {
            // Check for left mouse click
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Check if pointer is over any UI
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    actionBox.SetActive(false);
                    currentOpenActionBox = null;
                }
                else
                {
                    // If pointer is over UI, check if it's over the action box
                    var pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current)
                    {
                        position = UnityEngine.InputSystem.Mouse.current.position.ReadValue()
                    };
                    var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                    UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);

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
    public static void HideAllActionBoxes()
    {
        if (currentOpenActionBox != null)
        {
            currentOpenActionBox.SetActive(false);
            currentOpenActionBox = null;
        }
    }
    public void SetData(ItemStats data, int count, InventoryMenu menu)
    {
        itemData = data;
        menuRef = menu;

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
        menuRef.UseItem(itemData);
        actionBox.SetActive(false);
    }

    private void OnDropClicked()
    {
        menuRef.DropItem(itemData);
        actionBox.SetActive(false);
    }
}