using UnityEngine;

public class CharacterMovementController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float fallGravityMultiplier = 1.8f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Jump")]
    [SerializeField] private float coyoteTime = 0.12f;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb2D;
    private Vector2 rawMove;

    private bool movementLocked;

    private float lastGroundedTime;
    private bool hasConsumedCoyoteJump;

    public bool IsFacingLeft => sprite != null && sprite.flipX;
    public bool IsFacingRight => !IsFacingLeft;

    public bool IsMoving => Mathf.Abs(rawMove.x) > moveThreshold;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();
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

        bool isMoving = Mathf.Abs(rawMove.x) > moveThreshold;

        if (!isMoving || sprite == null)
            return;

        if (rawMove.x > 0)
            sprite.flipX = false;
        else if (rawMove.x < 0)
            sprite.flipX = true;
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
}