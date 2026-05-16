using System.Collections;
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
    [SerializeField] private string attackTrigger = "Punch";

    private bool isAttacking;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        
    }
    private void Update()
    {
        if (isAttacking)
        {
            if (characterMovement.IsGrounded())
            {
                OnKickFinished();
            }
        }
    }
    public override void TryExecute()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    
    public void OnKickStart()
    {
        GameObject hitBoxToEnable = GetCorrectHitBox();
        hitBoxToEnable.SetActive(true);
        characterMovement.SetMovementLocked(true);
        if (hitBoxToEnable == null)
            return;

        
    }

    public void OnKickFinished()
    {
        isAttacking = false;
        if(leftKickColliderObject != null)
            leftKickColliderObject.SetActive(false);
        if(rightKickColliderObject != null)
            rightKickColliderObject.SetActive(false);
        characterMovement.SetMovementLocked(false);

    }

    private GameObject GetCorrectHitBox()
    {
        if (characterMovement == null)
            return null;

        if (characterMovement.IsFacingLeft)
        {
            return leftKickColliderObject;
        }
        else
        {
            return rightKickColliderObject;
        }

        
    }

    

  
}
