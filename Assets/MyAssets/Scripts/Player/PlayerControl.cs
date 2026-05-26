using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private DashMovementController dashMovement;
    [SerializeField] private CharacterAttackController attackController;

    [Header("Attacks")]
    [SerializeField] private AttackBase punchAttack;
    [SerializeField] private AttackBase airKickAttack;
    [SerializeField] private AttackBase airGroundAttack;

    [Header("Input")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference punch;

    [Header("Input Thresholds")]
    [SerializeField] private float horizontalThreshold = 0.5f;
    [SerializeField] private float verticalThreshold = 0.5f;



    [SerializeField] Transform respawnPoint;

    private Vector2 moveInput;
    private bool hasAirDashed;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (dashMovement == null)
            dashMovement = GetComponent<DashMovementController>();

        if (attackController == null)
            attackController = GetComponent<CharacterAttackController>();
    }

    private void OnEnable()
    {
        if (move != null)
        {
            move.action.Enable();
            move.action.performed += OnMove;
            move.action.canceled += OnMove;
        }

        if (jump != null)
        {
            jump.action.Enable();
            jump.action.performed += OnJump;
            jump.action.canceled += OnJumpCanceled;
        }

        if (punch != null)
        {
            punch.action.Enable();
            punch.action.performed += OnPunch;
        }
    }

    private void OnDisable()
    {
        if (move != null)
        {
            move.action.performed -= OnMove;
            move.action.canceled -= OnMove;
            move.action.Disable();
        }

        if (jump != null)
        {
            jump.action.performed -= OnJump;
            jump.action.canceled -= OnJumpCanceled;
            jump.action.Disable();
        }

        if (punch != null)
        {
            punch.action.performed -= OnPunch;
            punch.action.Disable();
        }
    }

    private void Update()
    {
        if (characterMovement == null)
            return;

        if (characterMovement.IsGrounded())
            hasAirDashed = false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (characterMovement != null)
        {
            Vector2 movementInput = new Vector2(GetHorizontalMovementInput(), 0f);
            characterMovement.SetRawMove(movementInput);
        }
    }

    private float GetHorizontalMovementInput()
    {
        if (Mathf.Abs(moveInput.x) < horizontalThreshold)
            return 0f;

        return Mathf.Sign(moveInput.x);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (characterMovement == null)
            return;

        bool jumped = characterMovement.TryJump();

        if (jumped)
        {
            hasAirDashed = false;
            return;
        }

        TryAirDash(moveInput, false);
    }
    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        if (characterMovement == null)
            return;

        characterMovement.CutJump();
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        TryAttackFromInput();
    }

    private void TryAttackFromInput()
    {
        if (characterMovement == null)
            return;

        if (attackController == null)
            return;

        bool isGrounded = characterMovement.IsGrounded();

        bool wantsUp = moveInput.y > verticalThreshold;
        bool wantsDown = moveInput.y < -verticalThreshold;
        bool wantsSide = Mathf.Abs(moveInput.x) > horizontalThreshold;

        if (isGrounded)
        {
            TryGroundAttack(wantsUp, wantsDown);
            return;
        }

        TryAirAttack(wantsUp, wantsDown, wantsSide);
    }

    private void TryGroundAttack(bool wantsUp, bool wantsDown)
    {
        bool wantsUpperAttack = wantsUp || wantsDown;

        characterMovement.SetLookingUp(wantsUpperAttack);
        attackController.TryAttack(punchAttack);
    }

    private void TryAirAttack(bool wantsUp, bool wantsDown, bool wantsSide)
    {
        if (wantsDown)
        {
            characterMovement.SetLookingUp(false);
            attackController.TryAttack(airGroundAttack);
            return;
        }

        if (!hasAirDashed)
        {
            TryFirstAirAttack(wantsUp, wantsSide);
            return;
        }

        TryNormalAirAttack(wantsUp);
    }

    private void TryFirstAirAttack(bool wantsUp, bool wantsSide)
    {
        if (wantsSide)
        {
            float dashDirectionX = Mathf.Sign(moveInput.x);
            Vector2 dashDirection = new Vector2(dashDirectionX, 0f);

            bool dashStarted = TryAirDash(dashDirection, true);

            if (dashStarted)
            {
                characterMovement.SetLookingUp(false);
                attackController.TryAttack(airKickAttack);
            }

            return;
        }

        if (wantsUp)
        {
            Vector2 dashDirection = Vector2.up;

            bool dashStarted = TryAirDash(dashDirection, false);

            if (dashStarted)
            {
                characterMovement.SetLookingUp(true);
                attackController.TryAttack(punchAttack);
            }

            return;
        }

        characterMovement.SetLookingUp(false);
        attackController.TryAttack(punchAttack);
    }

    private void TryNormalAirAttack(bool wantsUp)
    {
        characterMovement.SetLookingUp(wantsUp);
        attackController.TryAttack(punchAttack);
    }

    private bool TryAirDash(Vector2 dashInput, bool onlyHorizontal)
    {
        if (dashMovement == null)
            return false;

        if (characterMovement == null)
            return false;

        if (characterMovement.IsGrounded())
            return false;

        bool dashStarted = dashMovement.TryDash(dashInput, onlyHorizontal);

        if (dashStarted)
            hasAirDashed = true;

        return dashStarted;
    }


    public void Respawn()
    {
        transform.position = respawnPoint.position;
        
    }
}