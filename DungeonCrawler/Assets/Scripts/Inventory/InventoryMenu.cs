using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{

    [Header("UI References")]
    public GameObject inventoryMenu; // Main inventory panel
    public TMP_Text CoinCountText;   // Coin display
    public Transform itemListParent; // VerticalLayoutGroup container
    public GameObject itemSlotPrefab; // Prefab with InventoryItemUI script

    [Header("Inventory Data")]
    public Inventory playerInventory;


    public int CoinCount;
    private ItemStats selectedItem;


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
                Time.timeScale = 0f; // Pause the game
                ShowInventoryContents();
            }
            else
            {
                Time.timeScale = 1f; // Resume the game
            }
        }

    }

    public void SelectItem(ItemStats item)
    {
        selectedItem = item;
        Debug.Log($"Now selected: {item.itemName}");
        // Show description, equip, drop, etc.
    }

    private void OnInventoryChanged()
    {
        Debug.Log("Inventory changed event received.");
        if (inventoryMenu.activeSelf)
            ShowInventoryContents();
    }

    void ShowInventoryContents()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Inventory not assigned.");
            return;
        }
        // Clear old UI entries
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        CoinCount = 0;

        var itemCounts = playerInventory.GetItemCounts();
        if (itemCounts.Count == 0)
        {
            // Show "empty" text slot if you want
            GameObject emptySlot = Instantiate(itemSlotPrefab, itemListParent);
            emptySlot.GetComponent<InventoryItemUI>().SetAsEmpty();
            return;
        }
        bool hasNonCoinItems = false;

        foreach (var kvp in itemCounts)
        {
            var item = kvp.Key;
            var count = kvp.Value;

            if (item.itemType == ItemStats.ItemType.Coin)
            {
                CoinCount += count;
                continue;
            }
            hasNonCoinItems = true;
            int maxStack = item.maxStack;
            while (count > 0)
            {
                int displayCount = Mathf.Min(count, maxStack);
                GameObject slotObj = Instantiate(itemSlotPrefab, itemListParent);
                slotObj.GetComponent<InventoryItemUI>().SetData(item, displayCount, this);
                count -= displayCount;
            }
        }

        if (!hasNonCoinItems)
        {
            GameObject emptySlot = Instantiate(itemSlotPrefab, itemListParent);
            emptySlot.GetComponent<InventoryItemUI>().SetAsEmpty();
        }

        if (CoinCountText != null)
            CoinCountText.text = $"Coins: {CoinCount}";
    }
    public void UseItem(ItemStats item)
    {
        if (item.itemType == ItemStats.ItemType.Consumable)
        {
            // Example: Heal player, etc.
            Debug.Log($"Used {item.itemName}");
            playerInventory.RemoveItem(item);
            ShowInventoryContents();
        }

    }

    public void DropItem(ItemStats item)
    {
        Debug.Log($"Dropped {item.itemName}");
        playerInventory.RemoveItem(item);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && item != null && item.itemPrefab != null)
        {
            Vector3 dropPosition = player.transform.position;
            GameObject dropped = Instantiate(item.itemPrefab, dropPosition, Quaternion.identity);

            // Set pickup delay if the prefab has ItemPickup
            var pickup = dropped.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.SetPickupDelay(2f); // 2 seconds delay
            }
        }
        else
        {
            Debug.LogWarning("Player, item, or pickupPrefab missing. Cannot drop item.");
        }
        ShowInventoryContents();
    }

}
