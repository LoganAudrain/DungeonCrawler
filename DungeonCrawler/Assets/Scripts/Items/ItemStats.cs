using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemStats", menuName = "Scriptable Objects/ItemStats")]
public class ItemStats : ScriptableObject
{
    public Sprite icon;
    [Range( 0 , 100)] public int HeathHealAmount;
    [Range(0, 100)] public int ManaHealAmount;
    [Range(1, 100)] public int maxStack;

}
