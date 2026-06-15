using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "CombatVehicle/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Visuals")]
    public string weaponName;
    public GameObject weaponPrefab; 
    [Header("Stats")]
    public float damage = 10f;
    public float fireRate = 0.5f; 
    public float range = 50f;

    [Header("Projectile")]
    public GameObject projectilePrefab; 
    public float projectileSpeed = 20f;
}
