using UnityEngine;

public enum WeaponType { Melee, Ranged }

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;
    public WeaponType type;
    public GameObject prefab;

    [Space(10)]
    [Header("Stats")]
    public int damage;
    public float attackRate;

    [Space(10)]
    [Header("Ranged-Specific")]
    public GameObject projectilePrefab;
    public float projectileSpeed;

    [Space(10)]
    [Header("Melee-Specific")]
    public float swingAngle;
    public float swingTime;
    public float thrustDistance;
    public float thrustTime;
    public float knockbackMultiplier;

    [Space(10)]
    [Header("Misc")]
    [Range(0f, 180f)] public float spawnTimeRotationX;
    [Range(0f, 180f)] public float spawnTimeRotationY;
    [Range(0f, 180f)] public float spawnTimeRotationZ;
}
