using UnityEngine;

public class AirKickAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private Animator animator;

    [Header("Hitboxes")]
    [SerializeField] private GameObject leftKickColliderObject;
    [SerializeField] private GameObject rightKickColliderObject;

    [Header("Animation")]
    [SerializeField] private string attackTrigger = "AirKick";

    private bool isAttacking;

    public override bool IsExecuting
    {
        get { return isAttacking; }
    }

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        DisableHitBoxes();
    }

    private void Update()
    {
        if (!isAttacking)
            return;

        if (characterMovement == null)
            return;

        if (characterMovement.IsGrounded())
            OnKickFinished();
    }

    public override void TryExecute()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        if (lifeManager != null)
            lifeManager.SetInvulnerable(true);

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    public void OnKickStart()
    {
        GameObject hitBoxToEnable = GetCorrectHitBox();

        if (hitBoxToEnable != null)
            hitBoxToEnable.SetActive(true);

        if (characterMovement != null)
            characterMovement.SetMovementLocked(true);
    }

    public void OnKickFinished()
    {
        isAttacking = false;

        DisableHitBoxes();

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);

        if (lifeManager != null)
            lifeManager.SetInvulnerable(false);
    }

    private GameObject GetCorrectHitBox()
    {
        if (characterMovement == null)
            return null;

        if (characterMovement.IsFacingLeft)
            return leftKickColliderObject;

        return rightKickColliderObject;
    }

    private void DisableHitBoxes()
    {
        if (leftKickColliderObject != null)
            leftKickColliderObject.SetActive(false);

        if (rightKickColliderObject != null)
            rightKickColliderObject.SetActive(false);
    }

    public override void ClearAttack()
    {
        DisableHitBoxes();

        isAttacking = false;

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);

        if (lifeManager != null)
            lifeManager.SetInvulnerable(false);
    }
}