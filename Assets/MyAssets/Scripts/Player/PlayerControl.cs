using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterMovementController2D characterMovement;
    [SerializeField] private DashMovementController dashController;
    [SerializeField] private CharacterAttackController attackController;
    [SerializeField] private AirKickAttack airKickAttack;
    [SerializeField] private AreaAirGroundAttack airGroundAttack;
    [SerializeField] private LifeManager lifeManager;

    [Header("Input")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference punch;

    public bool canMove = true;

    private Vector2 rawMove;
    private bool hasDashed;

    private void Awake()
    {
        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();

        if (dashController == null)
            dashController = GetComponent<DashMovementController>();

        if (attackController == null)
            attackController = GetComponent<CharacterAttackController>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();

        canMove = true;
        hasDashed = false;
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        punch.action.Enable();

        move.action.performed += OnMove;
        move.action.started += OnMove;
        move.action.canceled += OnMove;

        jump.action.performed += OnJump;
        punch.action.performed += OnPunch;

        if (lifeManager != null)
            lifeManager.OnLifeDepleted.AddListener(Die);
    }

    private void OnDisable()
    {
        move.action.performed -= OnMove;
        move.action.started -= OnMove;
        move.action.canceled -= OnMove;

        jump.action.performed -= OnJump;
        punch.action.performed -= OnPunch;

        move.action.Disable();
        jump.action.Disable();
        punch.action.Disable();

        if (lifeManager != null)
            lifeManager.OnLifeDepleted.RemoveListener(Die);
    }

    private void Update()
    {
        if (characterMovement == null)
            return;

        characterMovement.SetRawMove(rawMove);

        if (characterMovement.IsGrounded())
        {
            hasDashed = false;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (characterMovement == null)
            return;

        bool jumped = characterMovement.TryJump();

        if (jumped)
        {
            hasDashed = false;
            return;
        }

        TryAirDash(false, true);
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        if (attackController == null || characterMovement == null)
            return;

        if (characterMovement.IsGrounded())
        {
            hasDashed = false;
            attackController.TryDefaultAttack();
            return;
        }

        if (!hasDashed)
        {
            bool dashStarted = TryAirDash(true, false);

            if (dashStarted)
            {
                attackController.TryAttack(airKickAttack);
            }

            return;
        }

        attackController.TryAttack(airGroundAttack);
    }

    private bool TryAirDash(bool onlyHorizontal, bool countsAsDash)
    {
        if (dashController == null)
            return false;

        bool dashStarted = dashController.TryDash(rawMove, onlyHorizontal);

        if (dashStarted)
        {
            hasDashed = countsAsDash;
        }

        return dashStarted;
    }

    private void Die(float arg0)
    {
        gameObject.SetActive(false);
    }
}