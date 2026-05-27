using UnityEngine;

public class FlyingEnemyMovementController : EnemyMovementController
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Patrol")]
    [SerializeField] private bool canPatrol = true;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;

    [Header("Chase")]
    [SerializeField] private float chaseHorizontalSpeed = 1f;
    [SerializeField] private float chaseVerticalSpeed = 0.5f;
    [SerializeField] private float heightAboveTarget = 3f;

    [Header("Attack Position")]
    [SerializeField] private float maxHorizontalDistanceToAttack = 0.3f;
    [SerializeField] private float minHeightAboveTargetToAttack = 1.5f;

    [Header("Movement")]
    [SerializeField] private float positionTolerance = 0.05f;

    private Vector2 startPosition;
    private bool isGoingLeft = true;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
    }

    public override void UpdateIdleMovement()
    {
        if (!canPatrol)
        {
            Stop();
            return;
        }

        Patrol();
    }

    public override void MoveTowards(Transform target)
    {
        if (target == null)
        {
            UpdateIdleMovement();
            return;
        }

        Vector2 targetPosition = new Vector2(
            target.position.x,
            target.position.y + heightAboveTarget
        );

        MoveTowardsPosition(targetPosition, chaseHorizontalSpeed, chaseVerticalSpeed);
    }

    public override void Stop()
    {
        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;
    }

    public bool IsAboveTarget(Transform target)
    {
        if (target == null)
            return false;

        float horizontalDistance = Mathf.Abs(transform.position.x - target.position.x);
        float heightDifference = transform.position.y - target.position.y;

        if (horizontalDistance > maxHorizontalDistanceToAttack)
            return false;

        if (heightDifference < minHeightAboveTargetToAttack)
            return false;

        return true;
    }

    private void Patrol()
    {
        float leftX = startPosition.x - patrolDistance;
        float rightX = startPosition.x + patrolDistance;

        Vector2 targetPosition;

        if (isGoingLeft)
        {
            targetPosition = new Vector2(leftX, startPosition.y);

            if (transform.position.x <= leftX + positionTolerance)
                isGoingLeft = false;
        }
        else
        {
            targetPosition = new Vector2(rightX, startPosition.y);

            if (transform.position.x >= rightX - positionTolerance)
                isGoingLeft = true;
        }

        MoveTowardsPosition(targetPosition, patrolSpeed, patrolSpeed);
    }

    private void MoveTowardsPosition(Vector2 targetPosition, float horizontalSpeed, float verticalSpeed)
    {
        if (rb == null)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 difference = targetPosition - currentPosition;

        float velocityX = 0f;
        float velocityY = 0f;

        if (Mathf.Abs(difference.x) > positionTolerance)
            velocityX = Mathf.Sign(difference.x) * horizontalSpeed;

        if (Mathf.Abs(difference.y) > positionTolerance)
            velocityY = Mathf.Sign(difference.y) * verticalSpeed;

        rb.linearVelocity = new Vector2(velocityX, velocityY);
    }
}