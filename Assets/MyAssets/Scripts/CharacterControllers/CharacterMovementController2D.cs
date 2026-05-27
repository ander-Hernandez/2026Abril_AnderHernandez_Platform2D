using System.Collections;
using UnityEngine;

public class CharacterMovementController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;
    
    [Header("Sprite Direction")]
    [SerializeField] private bool spriteFacesRightByDefault = true;


    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Jump")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb2D;
    private Vector2 rawMove;
    public bool IsLookingUp;

    private bool movementLocked;

    private float lastGroundedTime;
    private bool hasConsumedCoyoteJump;
    private Coroutine knockbackCoroutine;

    [Header("Facing Lock")]
    [SerializeField] private float defaultFacingLockTime = 0.25f;
    private float facingLockedUntilTime;
    public bool IsFacingRight
    {
        get
        {
            if (sprite == null)
                return true;

            return spriteFacesRightByDefault ? !sprite.flipX : sprite.flipX;
        }
    }
    public bool IsFacingLeft => !IsFacingRight;

    public bool IsMoving => Mathf.Abs(rawMove.x) > moveThreshold;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();
        IsLookingUp = false;
    }

    private void Start()
    {
        if (animator != null)
            animator.SetFloat("Speed", moveSpeed);
    }

    private void Update()
    {

        
        UpdateGroundedState();

        UpdateMovement();
        UpdateSpriteDirection();
        UpdateAnimationParameters();
        FixFreeFall();
        
    }


    private void UpdateGroundedState()
    {
        if (IsGrounded())
        {
            lastGroundedTime = Time.time;
            hasConsumedCoyoteJump = false;
        }
    }

    private void FixFreeFall()
    {
        if (movementLocked)
            return;

        if (rb2D.linearVelocityY < 0f)
        {
            rb2D.linearVelocityY += Physics2D.gravity.y
                                 * (fallGravityMultiplier - 1f)
                                 * Time.deltaTime;
        }
    }

    public void SetRawMove(Vector2 inputRaw)
    {
        rawMove = inputRaw;
    }

    public void StopMovement()
    {
        rawMove = Vector2.zero;
        rb2D.linearVelocityX = 0f;
    }

    public bool TryJump()
    {
        if (movementLocked)
            return false;

        if (!CanJump())
            return false;

        rb2D.linearVelocityY = jumpForce;

        hasConsumedCoyoteJump = true;

        return true;
    }
    public void CutJump()
    {
        if (movementLocked)
            return;

        if (rb2D.linearVelocityY <= 0f)
            return;

        rb2D.linearVelocityY *= jumpCutMultiplier;
    }
    private bool CanJump()
    {
        if (IsGrounded())
            return true;

        bool isInsideCoyoteTime = Time.time <= lastGroundedTime + coyoteTime;

        return isInsideCoyoteTime && !hasConsumedCoyoteJump;
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayerMask
        );

        return hit.collider != null;
    }

    private void UpdateMovement()
    {
        
        if (movementLocked)
            return;

        rb2D.linearVelocityX = rawMove.x * moveSpeed;
    }

    private void UpdateSpriteDirection()
    {
        if (movementLocked)
            return;

        if (Time.time < facingLockedUntilTime)
            return;

        bool isMoving = Mathf.Abs(rawMove.x) > moveThreshold;

        if (!isMoving || sprite == null)
            return;

        if (rawMove.x > 0)
            sprite.flipX = !spriteFacesRightByDefault;
        else if (rawMove.x < 0)
            sprite.flipX = spriteFacesRightByDefault;
    }
    public void FaceTowards(Transform target)
    {
        FaceTowards(target, defaultFacingLockTime);
    }

    public void FaceTowards(Transform target, float lockTime)
    {
        if (target == null)
            return;

        float directionX = target.position.x - transform.position.x;

        FaceDirection(directionX, lockTime);
    }

    public void FaceDirection(float directionX, float lockTime)
    {
        if (sprite == null)
            return;

        if (Mathf.Abs(directionX) <= 0.01f)
            return;

        if (directionX > 0f)
            sprite.flipX = !spriteFacesRightByDefault;
        else
            sprite.flipX = spriteFacesRightByDefault;

        facingLockedUntilTime = Time.time + lockTime;
    }

    private void UpdateAnimationParameters()
    {
        if (animator == null)
            return;

        animator.SetBool("IsRunning", IsMoving);
        animator.SetBool("IsGrounded", IsGrounded());
    }

    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;
    }

    public void ApplyKnockback(Vector2 knockback, GameObject attacker, float duration)
    {
        if (attacker == null)
            return;

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(knockback, attacker, duration));
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockback, GameObject attacker, float duration)
    {
        movementLocked = true;

        float direction = Mathf.Sign(transform.position.x - attacker.transform.position.x);

        Vector2 finalKnockback = new Vector2(
            knockback.x * direction,
            knockback.y
        );

        rb2D.linearVelocity = finalKnockback;

        yield return new WaitForSecondsRealtime(duration);
        
        movementLocked = false;
        knockbackCoroutine = null;
    }

    public void SetLookingUp(bool lookingUp)
    {
        IsLookingUp = lookingUp;
    }


    
}