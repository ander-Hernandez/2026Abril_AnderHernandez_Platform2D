using UnityEngine;

public class RangedGroundEnemyMovementController : EnemyMovementController
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;

    [Header("Distance")]
    [SerializeField] private float preferredDistance = 3f;
    [SerializeField] private float distanceTolerance = 0.35f;

    [Header("Patrol")]
    [SerializeField] private bool canPatrol;
    [SerializeField] private float patrolDistance = 2f;

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

        MaintainDistanceFromTarget(target);
    }

    private void MaintainDistanceFromTarget(Transform target)
    {
        if (characterMovement == null)
            return;

        float distanceX = target.position.x - transform.position.x;
        float absoluteDistanceX = Mathf.Abs(distanceX);

        float minDistance = preferredDistance - distanceTolerance;
        float maxDistance = preferredDistance + distanceTolerance;

        if (absoluteDistanceX >= minDistance && absoluteDistanceX <= maxDistance)
        {
            Stop();
            return;
        }

        // Si está demasiado lejos, se acerca al jugador.
        if (absoluteDistanceX > maxDistance)
        {
            MoveInDirection(Mathf.Sign(distanceX));
            return;
        }

        // Si está demasiado cerca, se aleja del jugador.
        if (absoluteDistanceX < minDistance)
        {
            MoveInDirection(-Mathf.Sign(distanceX));
        }
    }

    private void Patrol()
    {
        float leftX = startPosition.x - patrolDistance;
        float rightX = startPosition.x + patrolDistance;

        if (isGoingLeft)
        {
            MoveTowardsX(leftX);

            if (transform.position.x <= leftX + 0.1f)
                isGoingLeft = false;

            return;
        }

        MoveTowardsX(rightX);

        if (transform.position.x >= rightX - 0.1f)
            isGoingLeft = true;
    }

    private void MoveTowardsX(float targetX)
    {
        float distanceX = targetX - transform.position.x;

        if (Mathf.Abs(distanceX) <= 0.1f)
        {
            Stop();
            return;
        }

        MoveInDirection(Mathf.Sign(distanceX));
    }

    private void MoveInDirection(float directionX)
    {
        if (characterMovement == null)
            return;

        if (directionX > 0f)
        {
            characterMovement.SetRawMove(Vector2.right);
            return;
        }

        if (directionX < 0f)
        {
            characterMovement.SetRawMove(Vector2.left);
            return;
        }

        Stop();
    }

    public override void Stop()
    {
        if (characterMovement == null)
            return;

        characterMovement.SetRawMove(Vector2.zero);
    }
}