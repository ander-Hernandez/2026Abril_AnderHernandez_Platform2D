using UnityEngine;

public class FlyingEnemyMovement : EnemyMovementBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 3f;
    [SerializeField] private float verticalSpeed = 2f;

    [Header("Idle Altitude")]
    [SerializeField] private float preferredWorldY = 5f;
    [SerializeField] private float idleReturnSpeed = 1.5f;

    [Header("Chase Position")]
    [SerializeField] private float heightAboveTarget = 3f;
    [SerializeField] private float stopDistance = 0.15f;

    [Header("Attack Position")]
    [SerializeField] private float maxHorizontalDistanceToAttack = 0.35f;
    [SerializeField] private float minHeightAboveTargetToAttack = 1.5f;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }


   
    public override void MoveTowards(Transform target)
    {
       
    }

    public override void Stop()
    {
       
    }

    public override void MoveTowardsPosition(Vector2 target)
    {
        throw new System.NotImplementedException();
    }
}