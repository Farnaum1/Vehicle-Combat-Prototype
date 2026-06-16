using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RocketProjectile : MonoBehaviour
{
    private float damage;
    private float speed;
    private TeamId ownerTeamId;
    private GameObject ownerObject;

    private bool initialized;

    private Collider[] projectileColliders;

    private void Awake()
    {
        projectileColliders = GetComponentsInChildren<Collider>();
    }

    public void Initialize(float damage, float speed, TeamId ownerTeamId, GameObject ownerObject)
    {
        this.damage = damage;
        this.speed = speed;
        this.ownerTeamId = ownerTeamId;
        this.ownerObject = ownerObject;

        initialized = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        IgnoreOwnerCollisions();

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!initialized)
            return;

        if (collision.gameObject == ownerObject)
            return;

        IDamageable damageable = collision.collider.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            Vector3 hitPoint = collision.contacts.Length > 0 ? collision.contacts[0].point : transform.position;

            DamageInfo damageInfo = new DamageInfo(damage, ownerTeamId, ownerObject, hitPoint);

            damageable.TakeDamage(damageInfo);
        }

        Destroy(gameObject);
    }

    private void IgnoreOwnerCollisions()
    {
        if (ownerObject == null)
            return;

        Collider[] ownerColliders = ownerObject.GetComponentsInChildren<Collider>();

        foreach (Collider projectileCol in projectileColliders)
        {
            foreach (Collider ownerCol in ownerColliders)
            {
                if (projectileCol != null && ownerCol != null)
                {
                    Physics.IgnoreCollision(projectileCol, ownerCol);
                }
            }
        }
    }
}
