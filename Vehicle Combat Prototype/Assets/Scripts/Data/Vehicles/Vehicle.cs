using UnityEngine;

public abstract class Vehicle : MonoBehaviour, IDamageable
{
    [Header("Data")]
    [SerializeField] private VehicleData vehicleData;

    [Header("Runtime")]
    [SerializeField] private TeamId teamId = TeamId.None;

    private float currentHealth;
    private bool isDestroyed;

    public VehicleData Data => vehicleData;
    public TeamId TeamId => teamId;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => vehicleData != null ? vehicleData.maxHealth : 0f;
    public bool IsDestroyed => isDestroyed;

    protected virtual void Awake()
    {
        InitializeFromData();
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

    public virtual void SetTeam(TeamId newTeamId)
    {
        teamId = newTeamId;
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

        GameEvents.RaiseVehicleDamaged (new VehicleDamagedEventArgs(gameObject, teamId, damageInfo, currentHealth, MaxHealth));

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

        GameEvents.RaiseVehicleDestroyed (new VehicleDestroyedEventArgs (gameObject, teamId, damageInfo.AttackerTeamId, damageInfo.Attacker));

        gameObject.SetActive(false);
    }
}
