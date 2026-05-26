using System.Collections;
using UnityEngine;

public class AreaAirGroundAttack : AttackBase
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Hitboxes")]
    [SerializeField] private GameObject impactColliderObject;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private float colliderTime = 0.1f;

    [Header("Animation")]
    [SerializeField] private string attackTrigger = "AirGroundAttack";

    [Header("Movement")]
    [SerializeField] private float attackMovementSpeed = 10f;
    [SerializeField] private float distanceToExplode = 1f;

    private Vector2 startingPosition;

    private bool isExecuting;
    private bool canExecute = true;
    private bool hasImpacted;

    public override bool IsExecuting
    {
        get { return isExecuting; }
    }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        isExecuting = false;
        canExecute = true;
        hasImpacted = false;

        if (impactColliderObject != null)
            impactColliderObject.SetActive(false);
    }

    private void Update()
    {
        if (!isExecuting)
            return;

        if (hasImpacted)
            return;

        if (characterMovement == null)
            return;

        if (characterMovement.IsGrounded())
        {
            hasImpacted = true;
            TriggerAttackHit();
        }
    }

    public override void TryExecute()
    {
        if (!canExecute)
            return;

        if (rb == null)
            return;

        canExecute = false;
        isExecuting = true;
        hasImpacted = false;

        if (characterMovement != null)
            characterMovement.SetMovementLocked(true);

        StartAttackDash();
    }

    private void StartAttackDash()
    {
        startingPosition = rb.position;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(0f, -attackMovementSpeed);

        if (animator != null)
            animator.SetTrigger(attackTrigger);
    }

    private void TriggerAttackHit()
    {
        if (!isExecuting)
            return;

        StartCoroutine(EnableAttackCollider());

        bool movedEnough = (rb.position - startingPosition).magnitude > distanceToExplode;

        if (movedEnough && impactPrefab != null)
        {
            GameObject instance = Instantiate(impactPrefab, transform.position, transform.rotation);
            Destroy(instance, colliderTime);
        }
    }

    private IEnumerator EnableAttackCollider()
    {
        if (impactColliderObject != null)
            impactColliderObject.SetActive(true);

        yield return new WaitForSeconds(colliderTime);

        EndAttack();
    }

    private void EndAttack()
    {
        if (impactColliderObject != null)
            impactColliderObject.SetActive(false);

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);

        isExecuting = false;
        canExecute = true;
        hasImpacted = false;
    }

    public override void ClearAttack()
    {
        if (impactColliderObject != null)
            impactColliderObject.SetActive(false);

        if (characterMovement != null)
            characterMovement.SetMovementLocked(false);

        isExecuting = false;
        canExecute = true;
        hasImpacted = false;
    }
}