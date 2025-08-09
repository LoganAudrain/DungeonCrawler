using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    public TMP_Text itemNameText;
    private ItemStats itemData;
    private InventoryMenu menuRef;

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
    }

    public void SetAsEmpty()
    {
        itemNameText.text = "Inventory is empty";
        GetComponent<Button>().interactable = false;
    }

    private void OnClick()
    {
        if (itemData != null)
            menuRef.SelectItem(itemData);
    }
}