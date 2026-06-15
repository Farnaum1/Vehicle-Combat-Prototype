using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleAIController : MonoBehaviour
{
    private Vehicle vehicle;

    [Header("AI")]
    public float detectionRange = 40f;
    public float attackRange = 25f;
    public float stopDistance = 10f;

    private Vehicle currentTarget;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Update()
    {
        if (vehicle.IsDestroyed)
            return;

        if (currentTarget == null || currentTarget.IsDestroyed)
        {
            FindTarget();
        }

        if (currentTarget == null)
            return;

        HandleMovement();
        HandleAttack();
    }

    void FindTarget()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        float bestDistance = Mathf.Infinity;
        Vehicle bestTarget = null;

        foreach (var v in vehicles)
        {
            if (v == vehicle)
                continue;

            if (v.TeamId == vehicle.TeamId)
                continue;

            if (v.IsDestroyed)
                continue;

            float dist = Vector3.Distance(transform.position, v.transform.position);

            if (dist < bestDistance && dist <= detectionRange)
            {
                bestDistance = dist;
                bestTarget = v;
            }
        }

        currentTarget = bestTarget;
    }

    void HandleMovement()
    {
        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                vehicle.Data.rotationSpeed * Time.deltaTime
            );

            transform.position += transform.forward *
                                  vehicle.Data.moveSpeed *
                                  Time.deltaTime;
        }
    }

    void HandleAttack()
    {
        float distance = Vector3.Distance(
            transform.position,
            currentTarget.transform.position
        );

        if (distance <= attackRange)
        {
            vehicle.FireWeapon();
        }
    }
}
