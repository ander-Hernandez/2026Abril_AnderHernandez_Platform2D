using System.Collections;
using UnityEngine;

public class DashMovementController : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashPowerX = 7f;
    [SerializeField] private float dashPowerY = 4f;
    [SerializeField] private float dashDuration = 0.18f;

    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private Rigidbody2D rb;

    private bool canDash = true;
    private bool isDashing;

    private float originalGravityScale;

    public bool IsDashing => isDashing;
    public bool CanDash => canDash;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        originalGravityScale = rb.gravityScale;

        canDash = true;
    }

    private void Update()
    {
        if (!canDash && characterMovement.IsGrounded() && !isDashing)
        {
            canDash = true;
        }
    }

    public bool TryDash(Vector2 movementInput, bool onlyHorizontal)
    {
        if (!canDash) return false;
        if (isDashing) return false;
        Vector2 dashVector = movementInput;
        if (onlyHorizontal)
            dashVector = new Vector2(dashVector.x, 0);
        StartCoroutine(DashCoroutine(dashVector, onlyHorizontal));
        return true;
    }

    private IEnumerator DashCoroutine(Vector2 movementInput, bool onlyHorizontal)
    {
        canDash = false;
        isDashing = true;

        characterMovement.SetMovementLocked(true);

        Vector2 dashDirection = Vector2.zero;

        Debug.Log(movementInput);
        if (movementInput.magnitude <= 0.1f)
        {
            if (!onlyHorizontal)
            {
                dashDirection = Vector2.up;
            }
            
        }
        else
        {
            dashDirection = movementInput.normalized;
            
        }

        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        if(dashDirection.y >=0.9f)
            dashDirection.y *= 0.7f;
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocityX = dashDirection.x * dashPowerX;
        rb.linearVelocityY = dashDirection.y * dashPowerY;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravityScale;

        isDashing = false;
        characterMovement.SetMovementLocked(false);
    }
}