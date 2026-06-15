using UnityEngine;

public abstract class Vehicle : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] private VehicleData vehicleData;

    [Header("Runtime")]
    [SerializeField] private TeamId teamId = TeamId.None;

    [Header("Weapon")]
    [SerializeField] private Transform weaponMountPoint;

    private float currentHealth;
    private bool isDestroyed;
    private Weapon currentWeapon;

    public VehicleData Data => vehicleData;
    public TeamId TeamId => teamId;
    public Weapon CurrentWeapon => currentWeapon;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => vehicleData != null ? vehicleData.maxHealth : 0f;
    public bool IsDestroyed => isDestroyed;

    protected virtual void Awake()
    {
        InitializeFromData();
        SpawnDefaultWeapon();
    }

    protected virtual void InitializeFromData()
    {
        if (vehicleData == null)
        {
            Debug.LogError($"{name} has no VehicleData assigned.", this);
            return;
        }

        currentHealth = vehicleData.maxHealth;
        isDestroyed = false;
    }

    protected virtual void SpawnDefaultWeapon()
    {
        if (vehicleData == null)
            return;

        if (vehicleData.defaultWeapon == null)
        {
            Debug.LogWarning($"{name} has no default weapon assigned.", this);
            return;
        }

        if (vehicleData.defaultWeapon.weaponPrefab == null)
        {
            Debug.LogWarning($"{vehicleData.defaultWeapon.name} has no weapon prefab assigned.", vehicleData.defaultWeapon);
            return;
        }

        Transform mount = weaponMountPoint != null ? weaponMountPoint : transform;

        GameObject weaponObject = Instantiate(vehicleData.defaultWeapon.weaponPrefab, mount.position, mount.rotation, mount);

        currentWeapon = weaponObject.GetComponent<Weapon>();

        if (currentWeapon == null)
        {
            Debug.LogError("Weapon prefab does not have a Weapon component.", weaponObject);
            return;
        }

        currentWeapon.Initialize(gameObject, teamId);
    }

    public virtual void SetTeam(TeamId newTeamId)
    {
        teamId = newTeamId;

        if (currentWeapon != null)
        {
            currentWeapon.Initialize(gameObject, teamId);
        }
    }

    public virtual void FireWeapon()
    {
        if (isDestroyed)
            return;

        if (currentWeapon == null)
        {
            Debug.LogWarning($"{name} has no current weapon.", this);
            return;
        }

        currentWeapon.TryFire();
    }

    public virtual void TakeDamage(DamageInfo damageInfo)
    {
        if (isDestroyed)
            return;

        if (damageInfo.Amount <= 0f)
            return;

        if (damageInfo.AttackerTeamId == teamId && teamId != TeamId.None)
            return;

        currentHealth -= damageInfo.Amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);

        GameEvents.RaiseVehicleDamaged(
            new VehicleDamagedEventArgs(
                gameObject,
                teamId,
                damageInfo,
                currentHealth,
                MaxHealth
            )
        );

        if (currentHealth <= 0f)
        {
            DestroyVehicle(damageInfo);
        }
    }

    public virtual void Repair(float amount)
    {
        if (isDestroyed)
            return;

        if (amount <= 0f)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
    }

    protected virtual void DestroyVehicle(DamageInfo damageInfo)
    {
        if (isDestroyed)
            return;

        isDestroyed = true;

        GameEvents.RaiseVehicleDestroyed(
            new VehicleDestroyedEventArgs(
                gameObject,
                teamId,
                damageInfo.AttackerTeamId,
                damageInfo.Attacker
            )
        );

        gameObject.SetActive(false);
    }
}
