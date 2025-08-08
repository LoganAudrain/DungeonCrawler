using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{
    public GameObject inventoryMenu;
    public TMP_Text inventoryText;
    public int CoinCount;
    public TMP_Text CoinCountText;
    public Inventory playerInventory;

    void Start()
    {
        inventoryMenu.SetActive(false);
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += OnInventoryChanged;
    }

    void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= OnInventoryChanged;
    }


    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            bool showMenu = !inventoryMenu.activeSelf;
            inventoryMenu.SetActive(showMenu);

            if (showMenu)
            {
                ShowInventoryContents();
            }
        }

    }

    private void OnInventoryChanged()
    {
        Debug.Log("Inventory changed event received.");
        if (inventoryMenu.activeSelf)
            ShowInventoryContents();
    }

    void ShowInventoryContents()
    {
        if (playerInventory == null || inventoryText == null)
        {
            Debug.LogError("Inventory or inventoryText not assigned.");
            return;
        }

        CoinCount = 0;

        var itemCounts = playerInventory.GetItemCounts();
        if (itemCounts.Count == 0)
        {
            inventoryText.text = "Inventory is empty.";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in itemCounts)
        {
            string itemName = kvp.Key.name;
            int count = kvp.Value;
            int maxStack = kvp.Key.maxStack;

            if (kvp.Key.itemType == ItemStats.ItemType.Coin)
            {
                CoinCount += count;

                continue;
            }

            while (count > 0)
            {
                int displayCount = Mathf.Min(count, maxStack);
                if (maxStack > 1)
                    sb.AppendLine($"{itemName} x{displayCount}");
                else
                    sb.AppendLine(itemName);

                count -= displayCount;
            }
        }
        inventoryText.text = sb.ToString();
        if (CoinCountText != null)
            CoinCountText.text = $"Coins: {CoinCount}";

    }

}
