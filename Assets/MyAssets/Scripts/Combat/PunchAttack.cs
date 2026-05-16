using System.Collections;
using UnityEngine;

public class PunchAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private Animator animator;

    [Header("Hitboxes")]
    [SerializeField] private GameObject rightPunchHitBox;
    [SerializeField] private GameObject leftPunchHitBox;
    [SerializeField] private GameObject rightRunningPunchHitBox;
    [SerializeField] private GameObject leftRunningPunchHitBox;
    [SerializeField] private float hitBoxTime = 0.1f;

    [Header("Animation")]
    [SerializeField] private string punchTrigger = "Punch";

    private bool isAttacking;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        DisableAllHitBoxes();
    }

    public override void TryExecute()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        if (animator != null)
            animator.SetTrigger(punchTrigger);
    }

    // Animation Event
    public void OnPunchHit()
    {
        GameObject hitBoxToEnable = GetCorrectHitBox();

        if (hitBoxToEnable == null)
            return;

        StartCoroutine(EnableHitBoxRoutine(hitBoxToEnable, hitBoxTime));
    }

    public void OnPunchFinished()
    {
        isAttacking = false;
    }

    private GameObject GetCorrectHitBox()
    {
        if (characterMovement == null)
            return null;

        if (characterMovement.IsFacingLeft)
        {
            return characterMovement.IsMoving
                ? leftRunningPunchHitBox
                : leftPunchHitBox;
        }

        return characterMovement.IsMoving
            ? rightRunningPunchHitBox
            : rightPunchHitBox;
    }

    private IEnumerator EnableHitBoxRoutine(GameObject hitBox, float time)
    {
        hitBox.SetActive(true);

        yield return new WaitForSeconds(time);

        hitBox.SetActive(false);
        OnPunchFinished();
        
    }

    private void DisableAllHitBoxes()
    {
        if (rightPunchHitBox != null)
            rightPunchHitBox.SetActive(false);

        if (leftPunchHitBox != null)
            leftPunchHitBox.SetActive(false);

        if (rightRunningPunchHitBox != null)
            rightRunningPunchHitBox.SetActive(false);

        if (leftRunningPunchHitBox != null)
            leftRunningPunchHitBox.SetActive(false);
    }
}