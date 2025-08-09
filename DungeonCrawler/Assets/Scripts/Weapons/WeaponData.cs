using UnityEngine;

public enum WeaponType { Melee, Ranged }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;
    public WeaponType type;
    public GameObject prefab;

    [Space(10)]
    [Header("Stats")]
    public float damage;
    public float attackRate;

    [Space(10)]
    [Header("Ranged-Specific")]
    public GameObject projectilePrefab;
    public float projectileSpeed;

    [Space(10)]
    [Header("Melee-Specific")]
    public float swingAngle;
    public float swingTime;
    public float knockback;
}
