using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private WeaponData weaponData;

    [Header("Runtime")]
    [SerializeField] private Transform firePoint;

    private GameObject ownerObject;
    private TeamId ownerTeamId = TeamId.None;
    private float nextFireTime;

    public WeaponData Data => weaponData;
    public GameObject OwnerObject => ownerObject;
    public TeamId OwnerTeamId => ownerTeamId;

    public bool CanFire => Time.time >= nextFireTime;

    public virtual void Initialize(GameObject owner, TeamId teamId)
    {
        ownerObject = owner;
        ownerTeamId = teamId;
    }

    public virtual void TryFire()
    {
        if (!CanFire)
            return;

        Fire();
        nextFireTime = Time.time + weaponData.fireRate;

        GameEvents.RaiseWeaponFired (new WeaponFiredEventArgs (gameObject, ownerObject, ownerTeamId));
    }

    protected abstract void Fire();

    protected DamageInfo CreateDamageInfo(Vector3 hitPoint)
    {
        return new DamageInfo (weaponData.damage, ownerTeamId, ownerObject, hitPoint);
    }

    protected Transform FirePoint
    {
        get
        {
            if (firePoint != null)
                return firePoint;

            return transform;
        }
    }
}