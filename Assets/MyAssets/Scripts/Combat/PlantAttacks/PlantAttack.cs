using UnityEngine;

public class PlantAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private EnemyAttackController enemyAttackController;
    [SerializeField] private Animator animator;

    [Header("Root")]
    [SerializeField] private GameObject rootPrefab;
    [SerializeField] private float rootTime = 2f;

    [Header("Animation")]
    [SerializeField] private string attackTrigger = "Punch";

    private bool isAttacking;
    private bool hasSpawnedRoot;

    public override bool IsExecuting
    {
        get { return isAttacking; }
    }

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (enemyAttackController == null)
            enemyAttackController = GetComponent<EnemyAttackController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        isAttacking = false;
        hasSpawnedRoot = false;
    }

    public override void TryExecute()
    {
        if (isAttacking)
            return;

        if (characterMovement == null)
            return;

        isAttacking = true;
        hasSpawnedRoot = false;

        characterMovement.SetMovementLocked(true);

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    // Animation Event
    public void TriggerAttackHit()
    {
        if (!isAttacking)
            return;

        if (hasSpawnedRoot)
            return;

        if (rootPrefab == null)
            return;

        hasSpawnedRoot = true;

        Vector2 attackVector = GetAttackDirection();

        GameObject instance = Instantiate(rootPrefab, transform.position, transform.rotation);

        MovingRootBehaviour movingRootBehaviour = instance.GetComponent<MovingRootBehaviour>();

        if (movingRootBehaviour != null)
            movingRootBehaviour.InitializeAttack(attackVector);
        OnPlantAttackAnimationEnds();

        Destroy(instance, rootTime);
    }

    public void OnPlantAttackAnimationEnds()
    {
        EndAttack();
    }

    private Vector2 GetAttackDirection()
    {
        Transform target = GetCurrentTarget();

        if (target != null)
        {
            if (target.position.x > transform.position.x)
                return Vector2.right;

            if (target.position.x < transform.position.x)
                return Vector2.left;
        }

        if (characterMovement != null && characterMovement.IsFacingRight)
            return Vector2.right;

        return Vector2.left;
    }

    private Transform GetCurrentTarget()
    {
        if (enemyAttackController == null)
            return null;

        return enemyAttackController.GetCurrentTarget();
    }

    private void EndAttack()
    {
        if (!isAttacking)
            return;

        isAttacking = false;

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);
    }

    public override void ClearAttack()
    {
        isAttacking = false;
        hasSpawnedRoot = false;

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);

        if (animator != null)
            animator.ResetTrigger(attackTrigger);
    }
}