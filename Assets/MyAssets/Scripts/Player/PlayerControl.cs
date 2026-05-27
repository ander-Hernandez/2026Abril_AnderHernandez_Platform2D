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
    [SerializeField] private InputActionReference heavyAttack;

    [Header("Input Thresholds")]
    [SerializeField] private float horizontalThreshold = 0.5f;
    [SerializeField] private float verticalThreshold = 0.5f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

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

        if (heavyAttack != null)
        {
            heavyAttack.action.Enable();
            heavyAttack.action.performed += OnHeavyAttack;
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

        if (heavyAttack != null)
        {
            heavyAttack.action.performed -= OnHeavyAttack;
            heavyAttack.action.Disable();
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
        TryLightAttackFromInput();
    }

    private void OnHeavyAttack(InputAction.CallbackContext context)
    {
        TryHeavyAttackFromInput();
    }

    private void TryLightAttackFromInput()
    {
        if (characterMovement == null)
            return;

        if (attackController == null)
            return;

        bool isGrounded = characterMovement.IsGrounded();

        bool wantsUp = WantsUp();
        bool wantsDown = WantsDown();

        if (isGrounded)
        {
            TryGroundPunch(wantsUp, wantsDown);
            return;
        }

        TryAirLightAttack(wantsUp, wantsDown);
    }

    private void TryHeavyAttackFromInput()
    {
        if (characterMovement == null)
            return;

        if (attackController == null)
            return;

        bool isGrounded = characterMovement.IsGrounded();

        bool wantsUp = WantsUp();
        bool wantsDown = WantsDown();
        bool wantsSide = WantsSide();

        if (isGrounded)
        {
            TryGroundPunch(wantsUp, wantsDown);
            return;
        }

        TryAirHeavyAttack(wantsUp, wantsDown, wantsSide);
    }

    private void TryGroundPunch(bool wantsUp, bool wantsDown)
    {
        bool wantsUpperAttack = wantsUp || wantsDown;

        characterMovement.SetLookingUp(wantsUpperAttack);
        attackController.TryAttack(punchAttack);
    }

    private void TryAirLightAttack(bool wantsUp, bool wantsDown)
    {
        if (wantsDown)
        {
            characterMovement.SetLookingUp(false);
            attackController.TryAttack(airGroundAttack);
            return;
        }

        characterMovement.SetLookingUp(wantsUp);
        attackController.TryAttack(punchAttack);
    }

    private void TryAirHeavyAttack(bool wantsUp, bool wantsDown, bool wantsSide)
    {
        if (wantsDown)
        {
            characterMovement.SetLookingUp(false);
            attackController.TryAttack(airGroundAttack);
            return;
        }

        if (wantsSide)
        {
            TryHeavyAirKick();
            return;
        }

        if (wantsUp)
        {
            characterMovement.SetLookingUp(true);
            attackController.TryAttack(punchAttack);
            return;
        }

        characterMovement.SetLookingUp(false);
        attackController.TryAttack(punchAttack);
    }

    private void TryHeavyAirKick()
    {
        if (!hasAirDashed)
        {
            float dashDirectionX = Mathf.Sign(moveInput.x);
            Vector2 dashDirection = new Vector2(dashDirectionX, 0f);

            bool dashStarted = TryAirDash(dashDirection, true);

            if (!dashStarted)
                return;
        }

        characterMovement.SetLookingUp(false);
        attackController.TryAttack(airKickAttack);
    }

    private bool WantsUp()
    {
        return moveInput.y > verticalThreshold;
    }

    private bool WantsDown()
    {
        return moveInput.y < -verticalThreshold;
    }

    private bool WantsSide()
    {
        return Mathf.Abs(moveInput.x) > horizontalThreshold;
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
        if (respawnPoint == null)
            return;

        transform.position = respawnPoint.position;
    }
    public void SetRespawnPoint(Transform newRespawnPoint)
    {
        if (newRespawnPoint == null)
            return;

        respawnPoint = newRespawnPoint;
    }
}