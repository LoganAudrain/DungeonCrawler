using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemStats", menuName = "Scriptable Objects/ItemStats")]
public class ItemStats : ScriptableObject
{
  
    public enum ItemType
    {
        Consumable,
        Coin,
        Weapon
    }
    public ItemType itemType;

    public Sprite icon;
    public GameObject itemPrefab; // Prefab for item representation in the world
    public string itemName;
    public string description;
    [Range( 0 , 100)] public int HeathHealAmount;
    [Range(0, 100)] public int ManaHealAmount;
    [Range(1, 100)] public int maxStack;

    [Space(10)]
    [Tooltip("If item type is not 'weapon', leave blank")]
    public WeaponData weaponData;


}
