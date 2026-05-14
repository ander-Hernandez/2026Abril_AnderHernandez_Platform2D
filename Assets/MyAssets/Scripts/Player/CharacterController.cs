using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float moveThreshold = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("References")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb2D;
    private Vector2 rawMove;

    public bool IsFacingLeft => sprite != null && sprite.flipX;
    public bool IsFacingRight => !IsFacingLeft;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        UpdateMovement();
        UpdateSpriteDirection();
        UpdateAnimations();
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

    public void Jump()
    {
        if (!IsGrounded()) return;

        rb2D.linearVelocityY = jumpForce;
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
        rb2D.linearVelocityX = rawMove.x * moveSpeed;
    }

    private void UpdateSpriteDirection()
    {
        bool isMoving = Mathf.Abs(rawMove.x) > moveThreshold;

        if (!isMoving || sprite == null) return;

        if (rawMove.x > 0)
            sprite.flipX = false;
        else if (rawMove.x < 0)
            sprite.flipX = true;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = Mathf.Abs(rawMove.x) > moveThreshold;

        animator.SetBool("IsRunning", isMoving);
        animator.SetBool("IsGrounded", IsGrounded());
    }
}