using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action OnInventoryChanged;
    [SerializeField] int maxSlots = 10;
    private class InventorySlot
    {
        public ItemStats itemStats;
        public int count;

        public InventorySlot(ItemStats stats, int initialCount)
        {
            itemStats = stats;
            count = initialCount;
        }
    }

    private List<InventorySlot> items = new List<InventorySlot>();

    public bool AddItem(ItemStats itemStats)
    {
        if (itemStats == null)
        {
            return false;
        }
        bool added = false;
        foreach (var slot in items)
        {
            if (slot.itemStats == itemStats && slot.count < itemStats.maxStack)
            {
                slot.count++;
                added = true;
                break;
            }
        }
        if (!added && items.Count < maxSlots)
        {
            items.Add(new InventorySlot(itemStats, 1));
            added = true;
        }
        if (added)
            OnInventoryChanged?.Invoke();

        return added;
    }

    public bool RemoveItem(ItemStats itemStats)
    {
        for (int i = 0; i < items.Count; i++)
        {
            var slot = items[i];
            if (slot.itemStats == itemStats)
            {
                slot.count--;
                if (slot.count <= 0)
                {
                    items.RemoveAt(i);
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public bool UseItemAtSlot(int slotIndex, CharacterStats characterStats)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
            return false;

        var slot = items[slotIndex];
        var item = slot.itemStats;

        if (item.itemType == ItemStats.ItemType.Consumable)
        {
            if (item.HeathHealAmount > 0 && characterStats != null)
            {
                int newHealth = characterStats.GetCurrentHealth + item.HeathHealAmount;
                if (newHealth > characterStats.GetMaxHealth)
                    newHealth = characterStats.GetMaxHealth;

                typeof(CharacterStats)
                    .GetField("m_currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(characterStats, newHealth);
            }
            if (item.ManaHealAmount > 0 && characterStats != null)
            {
                int newMana = characterStats.GetCurrentMana + item.ManaHealAmount;
                if (newMana > characterStats.GetMaxMana)
                    newMana = characterStats.GetMaxMana;
                typeof(CharacterStats)
                    .GetField("m_currentMana", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(characterStats, newMana);
            }

            RemoveItemAtSlot(slotIndex);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public bool DropItemAtSlot(int slotIndex, out ItemStats droppedItem)
    {
        droppedItem = null;
        if (slotIndex < 0 || slotIndex >= items.Count)
            return false;

        droppedItem = items[slotIndex].itemStats;
        RemoveItemAtSlot(slotIndex);
        OnInventoryChanged?.Invoke();
        return true;
    }

    private void RemoveItemAtSlot(int slotIndex)
    {
        var slot = items[slotIndex];
        slot.count--;
        if (slot.count <= 0)
        {
            items.RemoveAt(slotIndex);
        }
    }

    public IReadOnlyList<(ItemStats itemStats, int count)> GetSlots()
    {
        var result = new List<(ItemStats, int)>();
        foreach (var slot in items)
        {
            result.Add((slot.itemStats, slot.count));
        }
        return result;
    }

    public IReadOnlyDictionary<ItemStats, int> GetItemCounts()
    {
        Dictionary<ItemStats, int> counts = new Dictionary<ItemStats, int>();
        foreach (var slot in items)
        {
            if (slot.itemStats == null)
            {
                continue;
            }

            if (counts.ContainsKey(slot.itemStats))
                counts[slot.itemStats] += slot.count;
            else
                counts[slot.itemStats] = slot.count;
        }
        return counts;
    }
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= items.Count || indexB < 0 || indexB >= items.Count || indexA == indexB)
            return;

        var temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;
        OnInventoryChanged?.Invoke();
    }
}
