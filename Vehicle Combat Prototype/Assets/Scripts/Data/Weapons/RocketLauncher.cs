using UnityEngine;

public class RocketLauncher : Weapon
{
    protected override void Fire()
    {
        if (Data.projectilePrefab == null)
        {
            Debug.LogWarning($"{name} has no projectile prefab assigned.", this);
            return;
        }

        GameObject projectileObject = Instantiate(Data.projectilePrefab, FirePoint.position, FirePoint.rotation);

        RocketProjectile projectile = projectileObject.GetComponent<RocketProjectile>();

        if (projectile == null)
        {
            Debug.LogError("Projectile prefab does not have RocketProjectile component.", projectileObject);
            return;
        }

        projectile.Initialize(Data.damage, Data.projectileSpeed, OwnerTeamId, OwnerObject);
        Debug.DrawRay(FirePoint.position, FirePoint.forward * Data.range, Color.red, 0.2f);
    }
}
