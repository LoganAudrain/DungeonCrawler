using System.Collections;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] WeaponData weapon;

    public void UseWeapon(Vector2 aimDir)
    {
        if(weapon == null) return;

        switch (weapon.type)
        {
            case WeaponType.Melee:
            {
                UseMeleeWeapon(aimDir);
                break;
            }
            case WeaponType.Ranged:
            {
                UseRangedWeapon(aimDir);
                break;
            }
        }
    }

    public void ChangeWeapon(WeaponData _weapon)
    {
        weapon = _weapon;
    }

    private void UseMeleeWeapon(Vector2 aimDir)
    {
        if (weapon.prefab == null) return;
        Transform trans = GetComponent<Transform>();

        GameObject weaponInstance = Instantiate(
            weapon.prefab,
            trans.position,
            trans.rotation,
            trans
        );

        float startAngle = 0f;
        float endAngle = (aimDir.x >= 0) ? -weapon.swingAngle : weapon.swingAngle;

        StartCoroutine(SwingWeapon(weaponInstance.transform, startAngle, endAngle, weapon.swingTime));
    }

    private void UseRangedWeapon(Vector2 aimDir)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    { //because melee weapons are children of the player, the melee weapons collisions go here

        HealthComponent health = collision.GetComponent<HealthComponent>();
        if (health != null)
        {
            health.TakeDamage(weapon.damage);
        }
    }


    private IEnumerator SwingWeapon(Transform weaponTransform, float startAngle, float endAngle, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            weaponTransform.localRotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        Destroy(weaponTransform.gameObject);
    }
}
