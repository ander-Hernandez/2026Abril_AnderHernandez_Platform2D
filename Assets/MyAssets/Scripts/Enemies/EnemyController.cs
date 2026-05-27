using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private EnemyMovementController movementController;
    [SerializeField] private EnemyAttackController attackController;
    [SerializeField] private LifeManager lifeManager;

    [Header("Detection")]
    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private float loseTargetDistance = 9f;

    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 1f;

    private bool isDead;
    private bool hasDetectedTarget;
    private bool attackedThisFrame;

    private void Awake()
    {
        if (movementController == null)
            movementController = GetComponent<EnemyMovementController>();

        if (attackController == null)
            attackController = GetComponent<EnemyAttackController>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();
    }

    private void OnEnable()
    {
        isDead = false;
        hasDetectedTarget = false;

        if (lifeManager != null)
        {
            lifeManager.OnLifeChanged.AddListener(OnLifeChanged);
            lifeManager.OnLifeDepleted.AddListener(OnLifeDepleted);
        }
    }

    private void OnDisable()
    {
        if (lifeManager != null)
        {
            lifeManager.OnLifeChanged.RemoveListener(OnLifeChanged);
            lifeManager.OnLifeDepleted.RemoveListener(OnLifeDepleted);
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (attackController != null && attackController.IsAttackExecuting)
            return;

        attackedThisFrame = false;

        UpdateTargetDetection();
        UpdateEnemyAttack();
        UpdateEnemyMovement();
    }

    private void UpdateTargetDetection()
    {
        if (target == null)
        {
            hasDetectedTarget = false;
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (!hasDetectedTarget)
        {
            if (distanceToTarget <= detectionDistance)
                hasDetectedTarget = true;

            return;
        }

        if (distanceToTarget >= loseTargetDistance)
            hasDetectedTarget = false;
    }

    private void UpdateEnemyAttack()
    {
        if (!hasDetectedTarget)
            return;

        if (attackController == null)
            return;

        if (!attackController.CanAttack(target))
            return;

        attackController.TryAttack(target);
        attackedThisFrame = true;
    }

    private void UpdateEnemyMovement()
    {
        if (movementController == null)
            return;

        if (!hasDetectedTarget)
        {
            movementController.UpdateIdleMovement();
            return;
        }

        if (attackedThisFrame)
            return;

        movementController.MoveTowards(target);
    }

    private void OnLifeChanged(float currentLife, float startLife)
    {
        Debug.Log("Enemy took hit");
    }

    private void OnLifeDepleted(float startLife)
    {
        Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (movementController != null)
            movementController.Stop();

        if (attackController != null)
            attackController.ClearAttack();

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
        else
            gameObject.SetActive(false);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}