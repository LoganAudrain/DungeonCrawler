using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{

    [Header("UI References")]
    public GameObject inventoryMenu; // Main inventory panel
    public GameObject StatMenu; // Stats panel
    public TMP_Text CoinCountText;   // Coin display
    public TMP_Text CurrantHealthText; // Player health display
    public TMP_Text CurrantManaText; // Player mana display
    public TMP_Text MaxHealthText;  // Player max health display
    public TMP_Text MaxManaText; // Player max mana display
    public TMP_Text StrengthText; // Player strength display
    public TMP_Text ConstitutionText; // Player constitution display
    public TMP_Text DexterityText; // Player dexterity display
    public TMP_Text IntelligenceText; // Player intelligence display
    public Transform itemListParent; // VerticalLayoutGroup container
    public GameObject itemSlotPrefab; // Prefab with InventoryItemUI script

    [Header("Inventory Data")]
    public Inventory playerInventory;
    public CharacterStats CharacterStats;

    public int CoinCount;
    private ItemStats selectedItem;

    [Header("Stat Point System")]
    public int availableSP = 10; // Starting SP, adjust as needed
    public TMP_Text SPText; // Assign in inspector
    private int selectedStatIndex = 0;
    private readonly string[] statNames = { "Strength", "Constitution", "Dexterity", "Intelligence" };
    private readonly int minStatValue = 1; // Minimum allowed value for stats

    void Start()
    {
        inventoryMenu.SetActive(false);
        StatMenu.SetActive(false);
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
        if (inventoryMenu.activeSelf)
        {
            ShowInventoryContents();
            ShowStats();
            HandleStatPointInput();
        }
    }

    void HandleStatPointInput()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            selectedStatIndex = (selectedStatIndex - 1 + statNames.Length) % statNames.Length;
            ShowStats();
        }
        if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            selectedStatIndex = (selectedStatIndex + 1) % statNames.Length;
            ShowStats();
        }
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            TryIncreaseStat(selectedStatIndex);
        }
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            TryDecreaseStat(selectedStatIndex);
        }
    }

    // Call this from the ">" button for each stat, passing the stat index (0=Strength, 1=Constitution, etc.)
    public void OnIncreaseStatButton(int statIndex)
    {
        TryIncreaseStat(statIndex);
    }

    // Call this from the "<" button for each stat, passing the stat index
    public void OnDecreaseStatButton(int statIndex)
    {
        TryDecreaseStat(statIndex);
    }

    void TryIncreaseStat(int statIndex)
    {
        if (availableSP <= 0) return;

        switch (statIndex)
        {
            case 0: CharacterStats.IncreaseStrength(); break;
            case 1: CharacterStats.IncreaseConstitution(); break;
            case 2: CharacterStats.IncreaseDexterity(); break;
            case 3: CharacterStats.IncreaseIntelligence(); break;
        }
        availableSP--;
        ShowStats();
    }
    void TryDecreaseStat(int statIndex)
    {
       switch(statIndex)
        {
             case 0:
                if (CharacterStats.GetStrength > minStatValue)
                {
                     CharacterStats.DecreaseStrength();
                    availableSP++;
                }
                break;
            case 1:
                if (CharacterStats.GetConstitution > minStatValue)
                {
                    CharacterStats.DecreaseConstitution();
                    availableSP++;
                }
                break;
            case 2:
                if (CharacterStats.GetDexterity > minStatValue)
                {
                    CharacterStats.DecreaseDexterity();
                    availableSP++;
                }
                break;
            case 3:
                if (CharacterStats.GetIntelligence > minStatValue)
                {
                    CharacterStats.DecreaseIntelligence();
                    availableSP++;
                }
                break;
        }
       
        ShowStats();
    }
    public void SelectItem(ItemStats item)
    {
        selectedItem = item;
        Debug.Log($"Now selected: {item.itemName}");
    }

    private void OnInventoryChanged()
    {
        Debug.Log("Inventory changed event received.");
        if (inventoryMenu.activeSelf)
            ShowInventoryContents();
    }

    void ShowStats()
    {
        if (CharacterStats == null)
        {
            Debug.LogError("CharacterStats not assigned.");
            return;
        }
        CurrantHealthText.text = $"{CharacterStats.GetCurrentHealth}";
        MaxHealthText.text = $"{CharacterStats.GetMaxHealth}";
        CurrantManaText.text = $"{CharacterStats.GetCurrentMana}";
        MaxManaText.text = $"{CharacterStats.GetMaxMana}";
        StrengthText.text = CharacterStats.GetStrength.ToString();
        ConstitutionText.text = CharacterStats.GetConstitution.ToString();
        DexterityText.text = CharacterStats.GetDexterity.ToString();
        IntelligenceText.text = CharacterStats.GetIntelligence.ToString();

        if (SPText != null)
            SPText.text = $"{availableSP}";
    }

    void ShowInventoryContents()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Inventory not assigned.");
            return;
        }
        foreach (Transform child in itemListParent)
            Destroy(child.gameObject);

        CoinCount = 0;

        var slots = playerInventory.GetSlots();
        if (slots.Count == 0)
        {
            GameObject emptySlot = Instantiate(itemSlotPrefab, itemListParent);
            emptySlot.GetComponent<InventoryItemUI>().SetAsEmpty();
            return;
        }
        bool hasNonCoinItems = false;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            var item = slot.itemStats;
            var count = slot.count;

            if (item.itemType == ItemStats.ItemType.Coin)
            {
                CoinCount += count;
                continue;
            }
            hasNonCoinItems = true;
            GameObject slotObj = Instantiate(itemSlotPrefab, itemListParent);
            slotObj.GetComponent<InventoryItemUI>().SetData(item, count, this, i); // Pass slot index
        }

        if (!hasNonCoinItems)
        {
            GameObject emptySlot = Instantiate(itemSlotPrefab, itemListParent);
            emptySlot.GetComponent<InventoryItemUI>().SetAsEmpty();
        }

        if (CoinCountText != null)
            CoinCountText.text = $"Coins: {CoinCount}";
    }

 
    public void UseItemAtSlot(int slotIndex)
    {
        if (playerInventory.UseItemAtSlot(slotIndex, CharacterStats))
        {
            ShowStats();
            ShowInventoryContents();
        }
    }
    public void DropItemAtSlot(int slotIndex)
    {
        if (playerInventory.DropItemAtSlot(slotIndex, out ItemStats droppedItem))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && droppedItem != null && droppedItem.itemPrefab != null)
            {
                Vector3 dropPosition = player.transform.position;
                GameObject dropped = Instantiate(droppedItem.itemPrefab, dropPosition, Quaternion.identity);

                var pickup = dropped.GetComponent<ItemPickup>();
                if (pickup != null)
                {
                    pickup.SetPickupDelay(2f);
                }
            }
            else
            {
                Debug.LogWarning("Player, item, or pickupPrefab missing. Cannot drop item.");
            }
            ShowInventoryContents();
        }
    }
 public void SwapSlots(int indexA, int indexB)
{
    playerInventory.SwapSlots(indexA, indexB);
    ShowInventoryContents();
}

}
