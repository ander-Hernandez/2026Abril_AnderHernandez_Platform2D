using UnityEngine;

public class GroundEnemyMovement : EnemyMovementBehaviour
{
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private float distanceToStop;
    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();
    }

    public override void MoveTowards(Transform target)
    {
        if (target == null)
        {
            Stop();
            return;
        }

        if (characterMovement == null)
            return;

        Vector2 rawMove = Vector2.zero;
        if (Mathf.Abs(target.position.x - transform.position.x) < distanceToStop)
        {
            Stop();
            return;
        }
            

        if (target.position.x > transform.position.x)
            rawMove = Vector2.right;
        else if (target.position.x < transform.position.x)
            rawMove = Vector2.left;
        

            characterMovement.SetRawMove(rawMove);
    }

    public override void Stop()
    {
        if (characterMovement == null)
            return;

        characterMovement.SetRawMove(Vector2.zero);
    }

    public override void MoveTowardsPosition(Vector2 target)
    {
        throw new System.NotImplementedException();
    }
}