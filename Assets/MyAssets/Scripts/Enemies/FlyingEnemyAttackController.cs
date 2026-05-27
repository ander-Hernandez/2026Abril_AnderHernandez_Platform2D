using UnityEngine;

public class FlyingEnemyAttackController : EnemyAttackController
{
    [Header("Flying References")]
    [SerializeField] private FlyingEnemyMovementController flyingMovement;

    protected override void Awake()
    {
        base.Awake();

        if (flyingMovement == null)
            flyingMovement = GetComponent<FlyingEnemyMovementController>();
    }

    public override bool CanAttack(Transform target)
    {
        if (target == null)
            return false;

        if (!IsCooldownReady())
            return false;

        if (flyingMovement == null)
            return false;

        return flyingMovement.IsAboveTarget(target);
    }
}