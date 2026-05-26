using UnityEngine;

public class PlantAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private Animator animator;

    [Header("Root")]
    [SerializeField] private GameObject rootPrefab;
    [SerializeField] private float rootTime;

    [Header("Animation")]
    [SerializeField] private string attackTrigger = "Punch";

    private bool isAttacking;
    private bool hasSpawnedRoot;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

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
    private void TriggerAttackHit()
    {
        if (!isAttacking)
            return;

        if (hasSpawnedRoot)
            return;

        if (rootPrefab == null)
            return;

        hasSpawnedRoot = true;

        GameObject instance = Instantiate(rootPrefab, transform.position, transform.rotation);

        Vector2 attackVector = Vector2.left;

        if (characterMovement != null && characterMovement.IsFacingRight)
            attackVector = Vector2.right;

        MovingRootBehaviour movingRootBehaviour = instance.GetComponent<MovingRootBehaviour>();

        if (movingRootBehaviour != null)
            movingRootBehaviour.InitializeAttack(attackVector);

        Destroy(instance, rootTime);

        EndAttack();
    }

    public void OnPlantAttackAnimationEnds()
    {
        EndAttack();
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