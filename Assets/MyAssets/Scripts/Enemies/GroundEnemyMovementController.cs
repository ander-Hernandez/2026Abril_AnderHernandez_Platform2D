using UnityEngine;

public class GroundEnemyMovementController : EnemyMovementController
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;

    [Header("Movement")]
    [SerializeField] private float distanceToStop = 0.1f;

    [Header("Patrol")]
    [SerializeField] private bool canPatrol;
    [SerializeField] private float patrolDistance = 2f;

    [Header("Ledge Detection")]
    [SerializeField] private bool avoidLedges = true;
    [SerializeField] private Transform groundCheckLeft;
    [SerializeField] private Transform groundCheckRight;
    [SerializeField] private float ledgeCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundLayerMask;

    private Vector2 startPosition;
    private bool isGoingLeft = true;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

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

        MoveTowardsX(target.position.x);
    }

    private void Patrol()
    {
        float leftX = startPosition.x - patrolDistance;
        float rightX = startPosition.x + patrolDistance;

        if (isGoingLeft)
        {
            bool moved = MoveTowardsX(leftX);

            if (!moved || transform.position.x <= leftX + distanceToStop)
                isGoingLeft = false;

            return;
        }

        bool movedRight = MoveTowardsX(rightX);

        if (!movedRight || transform.position.x >= rightX - distanceToStop)
            isGoingLeft = true;
    }

    private bool MoveTowardsX(float targetX)
    {
        if (characterMovement == null)
            return false;

        float distanceX = targetX - transform.position.x;

        if (Mathf.Abs(distanceX) <= distanceToStop)
        {
            Stop();
            return false;
        }

        float directionX = Mathf.Sign(distanceX);
        return MoveInDirection(directionX);
    }

    private bool MoveInDirection(float directionX)
    {
        if (characterMovement == null)
            return false;

        if (directionX > 0f)
        {
            if (!CanMoveRight())
            {
                Stop();
                return false;
            }

            characterMovement.SetRawMove(Vector2.right);
            return true;
        }

        if (directionX < 0f)
        {
            if (!CanMoveLeft())
            {
                Stop();
                return false;
            }

            characterMovement.SetRawMove(Vector2.left);
            return true;
        }

        Stop();
        return false;
    }

    private bool CanMoveLeft()
    {
        if (!avoidLedges)
            return true;

        return HasGroundAhead(groundCheckLeft);
    }

    private bool CanMoveRight()
    {
        if (!avoidLedges)
            return true;

        return HasGroundAhead(groundCheckRight);
    }

    private bool HasGroundAhead(Transform checkPoint)
    {
        if (checkPoint == null)
            return true;

        RaycastHit2D hit = Physics2D.Raycast(
            checkPoint.position,
            Vector2.down,
            ledgeCheckDistance,
            groundLayerMask
        );

        return hit.collider != null;
    }

    public override void Stop()
    {
        if (characterMovement == null)
            return;

        characterMovement.SetRawMove(Vector2.zero);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (groundCheckLeft != null)
        {
            Gizmos.DrawLine(
                groundCheckLeft.position,
                groundCheckLeft.position + Vector3.down * ledgeCheckDistance
            );
        }

        if (groundCheckRight != null)
        {
            Gizmos.DrawLine(
                groundCheckRight.position,
                groundCheckRight.position + Vector3.down * ledgeCheckDistance
            );
        }
    }
}