using UnityEngine;

public class PuncherAIControll : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private CharacterMovementController2D characterMovementController;
    [SerializeField] private CharacterAttackController attackController;
    [SerializeField] private LifeManager lifeManager;

    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float distanceToStop = 0.05f;

    private float nextAttackTime;

    private void Awake()
    {
        if (characterMovementController == null)
            characterMovementController = GetComponent<CharacterMovementController2D>();

        if (attackController == null)
            attackController = GetComponent<CharacterAttackController>();

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
        

        if (characterMovementController != null)
        {
            if (target != null)
            {
                float distanceToTarget = Mathf.Abs(target.position.x - transform.position.x);

                if (distanceToTarget <= attackDistance && Time.time >= nextAttackTime)
                {
                    rawMove = Vector2.zero;
                    TryPunch();
                } else if(distanceToTarget <= distanceToStop)
                {
                    rawMove = Vector2.zero;
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
            characterMovementController.SetRawMove(rawMove);
        }
    }

    private void TryPunch()
    {
        if (attackController == null) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;

        attackController.TryDefaultAttack();
    }

}