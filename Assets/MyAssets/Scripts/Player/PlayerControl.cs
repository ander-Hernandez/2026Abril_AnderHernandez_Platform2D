using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AttackController attackController;
    [SerializeField] private LifeManager lifeManager;

    [Header("Input")]
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference punch;

    [Header("Player Hitboxes")]
    [SerializeField] private GameObject rightPunchHitBox;
    [SerializeField] private GameObject leftPunchHitBox;
    [SerializeField] private float punchHitBoxTime = 0.1f;
    [SerializeField] private GameObject movingAttackHitBox;
    [SerializeField] private float movingAttackHitBoxTime;

    private bool canMove;
 

    private Vector2 rawMove;
    
    

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (attackController == null)
            attackController = GetComponent<AttackController>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();

        move.action.performed += OnMove;
        move.action.started += OnMove;
        move.action.canceled += OnMove;

        jump.action.performed += OnJump;
        punch.action.performed += OnPunch;
        canMove = true;
        
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        punch.action.Enable();

        lifeManager.OnLifeDepleted.AddListener(Die);
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        punch.action.Disable();

        lifeManager.OnLifeDepleted.RemoveListener(Die);
    }

    private void Update()
    {
        characterController.SetRawMove(rawMove);
        if (characterController.IsGrounded())
        {
            canMove = true;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        characterController.Jump();
    }

    private void OnPunch(InputAction.CallbackContext context)
    {
        attackController.PlayAttack("Punch");
    }

    // Animation Event
    public void OnPunchHit()
    {
        GameObject hitBoxToEnable;

        if (characterController.IsFacingLeft)
        {
            hitBoxToEnable = leftPunchHitBox;
        }
        else
        {
            hitBoxToEnable = rightPunchHitBox;
        }

        attackController.EnableHitBox(hitBoxToEnable, punchHitBoxTime);

    }

    public void OnMovingAttackHit()
    {
        attackController.EnableHitBox(movingAttackHitBox, movingAttackHitBoxTime);
    }

    private void Die(float arg0)
    {
        gameObject.SetActive(false);
    }
}