using UnityEngine;

public class AirKickAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private Animator animator;

    [Header("Hitboxes")]
    [SerializeField] private GameObject leftKickColliderObject;
    [SerializeField] private GameObject rightKickColliderObject;

    [Header("Animation")]
    [SerializeField] private string attackTrigger = "AirKick";

    private bool isAttacking;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

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

        // Si toca el suelo mientras hace airkick, se corta.
        if (characterMovement.IsGrounded())
            OnKickFinished();
    }

    public override void TryExecute()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    // Animation Event
    public void OnKickStart()
    {
        GameObject hitBoxToEnable = GetCorrectHitBox();

        if (hitBoxToEnable == null)
            return;

        hitBoxToEnable.SetActive(true);

        if (characterMovement != null)
            characterMovement.SetMovementLocked(true);
    }

    // Animation Event, o llamada desde Update al tocar suelo
    public void OnKickFinished()
    {
        isAttacking = false;

        DisableHitBoxes();

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);
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
    }
}