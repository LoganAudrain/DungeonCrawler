using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event Action OnInventoryChanged;
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
        if (!added && items.Count < 10)
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

    // Optional: expose inventory contents
    public IReadOnlyList<ItemStats> GetItems()
    {
        List<ItemStats> result = new List<ItemStats>();
        foreach (var slot in items)
        {
            for (int i = 0; i < slot.count; i++)
                result.Add(slot.itemStats);
        }
        return result;
    }

    public IReadOnlyDictionary<ItemStats, int> GetItemCounts()
    {
        Dictionary<ItemStats, int> counts = new Dictionary<ItemStats, int>();
        foreach (var slot in items)
        {
            if (counts.ContainsKey(slot.itemStats))
                counts[slot.itemStats] += slot.count;
            else
                counts[slot.itemStats] = slot.count;
        }
        return counts;
    }
}
