using UnityEngine;

[CreateAssetMenu(fileName = "NewLootTable", menuName = "Items/Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class LootEntry
    {
        public GameObject itemPrefab;
        [Range(0, 1)]
        public float dropChance = 1.0f; // Probability for this item
    }

    public LootEntry[] lootEntries;

    public LootEntry GetRandomEntry()
    {
        float total = 0f;
        foreach (var entry in lootEntries) total += entry.dropChance;
        float roll = Random.value * total;
        float accum = 0f;
        foreach (var entry in lootEntries)
        {
            accum += entry.dropChance;
            if (roll <= accum) return entry;
        }
        return lootEntries.Length > 0 ? lootEntries[0] : null;
    }
}
