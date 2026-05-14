using UnityEngine;

public class PuncherAIControll : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private AttackController attackController;
    [SerializeField] private LifeManager lifeManager;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Punch Hitboxes")]
    [SerializeField] private GameObject rightPunchHitBox;
    [SerializeField] private GameObject leftPunchHitBox;
    [SerializeField] private float punchHitBoxTime = 0.1f;

    private float nextAttackTime;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (attackController == null)
            attackController = GetComponent<AttackController>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();
    }

    private void OnEnable()
    {
        lifeManager.OnLifeChanged.AddListener(OnLifeChanged);
        lifeManager.OnLifeDepleted.AddListener(OnLifeDepleted);
    }

    private void OnDisable()
    {
        lifeManager.OnLifeChanged.RemoveListener(OnLifeChanged);
        lifeManager.OnLifeDepleted.RemoveListener(OnLifeDepleted);
    }

    private void OnLifeChanged(float arg0, float arg1)
    {
        Debug.Log("Enemy took hit");
    }

    private void OnLifeDepleted(float arg0)
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector2 rawMove = Vector2.zero;

        if (characterController != null)
        {
            if (target != null)
            {
                float distanceToTarget = Mathf.Abs(target.position.x - transform.position.x);

                if (distanceToTarget <= attackDistance)
                {
                    rawMove = Vector2.zero;
                    TryPunch();
                }
                else
                {
                    if (target.position.x > transform.position.x)
                    {
                        rawMove = Vector2.right;
                    }

                    if (target.position.x < transform.position.x)
                    {
                        rawMove = Vector2.left;
                    }
                }
            }

            characterController.SetRawMove(rawMove);
        }
    }

    private void TryPunch()
    {
        if (attackController == null) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

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
}