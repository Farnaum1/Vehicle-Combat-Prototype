using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleAIController : MonoBehaviour
{
    private Vehicle vehicle;
    private Vehicle currentTarget;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 50f;
    [SerializeField] private float targetRefreshInterval = 1f;

    [Header("Combat Distance")]
    [SerializeField] private float attackRange = 30f;
    [SerializeField] private float preferredDistance = 18f;
    [SerializeField] private float tooCloseDistance = 10f;

    [Header("Movement")]
    [SerializeField] private float strafeWeight = 0.8f;
    [SerializeField] private float dodgeChangeInterval = 1.2f;
    [SerializeField] private float movementNoise = 0.25f;

    [Header("Cover")]
    [SerializeField] private bool useCoverWhenLowHealth = true;
    [SerializeField] private float lowHealthPercent = 0.35f;
    [SerializeField] private float coverSearchRange = 35f;
    [SerializeField] private float coverReachDistance = 3f;

    private float nextTargetRefreshTime;
    private float nextDodgeChangeTime;

    private int strafeDirection = 1;
    private Vector3 randomMovementOffset;

    private Transform currentCoverPoint;
    private bool movingToCover;

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        PickNewDodgeDirection();
    }

    private void Update()
    {
        if (vehicle.IsDestroyed)
            return;

        RefreshTargetIfNeeded();

        if (currentTarget == null)
        {
            IdleRotate();
            return;
        }

        if (ShouldSeekCover())
        {
            HandleCoverMovement();
            return;
        }

        HandleCombatMovement();
        HandleAttack();
    }

    private void RefreshTargetIfNeeded()
    {
        if (Time.time < nextTargetRefreshTime && currentTarget != null && !currentTarget.IsDestroyed)
            return;

        nextTargetRefreshTime = Time.time + targetRefreshInterval;
        FindTarget();
    }

    private void FindTarget()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        float bestDistance = Mathf.Infinity;
        Vehicle bestTarget = null;

        foreach (Vehicle candidate in vehicles)
        {
            if (candidate == vehicle)
                continue;

            if (candidate.IsDestroyed)
                continue;

            if (candidate.TeamId == vehicle.TeamId && vehicle.TeamId != TeamId.None)
                continue;

            float distance = Vector3.Distance(transform.position, candidate.transform.position);

            if (distance <= detectionRange && distance < bestDistance)
            {
                bestDistance = distance;
                bestTarget = candidate;
            }
        }

        currentTarget = bestTarget;
    }

    private void HandleCombatMovement()
    {
        Vector3 toTarget = currentTarget.transform.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.01f)
            return;

        float distance = toTarget.magnitude;
        Vector3 targetDirection = toTarget.normalized;

        RotateTowards(targetDirection);

        if (Time.time >= nextDodgeChangeTime)
        {
            PickNewDodgeDirection();
        }

        Vector3 moveDirection = Vector3.zero;

        if (distance > preferredDistance)
        {
            moveDirection += targetDirection;
        }
        else if (distance < tooCloseDistance)
        {
            moveDirection -= targetDirection;
        }

        if (distance <= attackRange)
        {
            Vector3 strafeDirectionVector = Vector3.Cross(Vector3.up, targetDirection).normalized;
            moveDirection += strafeDirectionVector * strafeDirection * strafeWeight;
        }

        moveDirection += randomMovementOffset;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Move(moveDirection.normalized);
        }
    }

    private void HandleAttack()
    {
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= attackRange)
        {
            vehicle.FireWeapon();
        }
    }

    private bool ShouldSeekCover()
    {
        if (!useCoverWhenLowHealth)
            return false;

        if (vehicle.MaxHealth <= 0)
            return false;

        float healthPercent = vehicle.CurrentHealth / vehicle.MaxHealth;

        if (healthPercent > lowHealthPercent)
        {
            movingToCover = false;
            currentCoverPoint = null;
            return false;
        }

        if (currentCoverPoint == null)
        {
            currentCoverPoint = FindBestCoverPoint();
        }

        movingToCover = currentCoverPoint != null;
        return movingToCover;
    }

    private void HandleCoverMovement()
    {
        if (currentCoverPoint == null)
        {
            movingToCover = false;
            return;
        }

        Vector3 toCover = currentCoverPoint.position - transform.position;
        toCover.y = 0f;

        float distanceToCover = toCover.magnitude;

        // هنوز به cover نرسیده
        if (distanceToCover > coverReachDistance)
        {
            RotateTowards(toCover.normalized);
            Move(toCover.normalized);
            return;
        }

        if (currentTarget == null)
            return;

        Vector3 toEnemy = currentTarget.transform.position - transform.position;
        toEnemy.y = 0f;

        RotateTowards(toEnemy.normalized);

        float distanceToEnemy = toEnemy.magnitude;

 
        Vector3 strafe = Vector3.Cross(Vector3.up, toEnemy.normalized) * strafeDirection * 0.6f;

        Move(strafe);

        if (distanceToEnemy <= attackRange)
        {
            vehicle.FireWeapon();
        }
    }
    private Transform FindBestCoverPoint()
    {
        CoverPoint[] coverPoints = FindObjectsOfType<CoverPoint>();

        Transform bestCover = null;
        float bestScore = Mathf.NegativeInfinity;

        foreach (CoverPoint coverPoint in coverPoints)
        {
            float distanceToCover = Vector3.Distance(transform.position, coverPoint.transform.position);

            if (distanceToCover > coverSearchRange)
                continue;

            float score = -distanceToCover;

            if (currentTarget != null)
            {
                float distanceFromEnemy = Vector3.Distance(
                    currentTarget.transform.position,
                    coverPoint.transform.position
                );

                score += distanceFromEnemy * 0.5f;
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestCover = coverPoint.transform;
            }
        }

        return bestCover;
    }

    private void PickNewDodgeDirection()
    {
        strafeDirection = Random.value > 0.5f ? 1 : -1;

        randomMovementOffset = new Vector3(
            Random.Range(-movementNoise, movementNoise),
            0f,
            Random.Range(-movementNoise, movementNoise)
        );

        nextDodgeChangeTime = Time.time + Random.Range(
            dodgeChangeInterval * 0.7f,
            dodgeChangeInterval * 1.4f
        );
    }

    private void IdleRotate()
    {
        transform.Rotate(Vector3.up, vehicle.Data.rotationSpeed * 0.25f * Time.deltaTime);
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            vehicle.Data.rotationSpeed * Time.deltaTime
        );
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction * vehicle.Data.moveSpeed * Time.deltaTime;
    }
}
