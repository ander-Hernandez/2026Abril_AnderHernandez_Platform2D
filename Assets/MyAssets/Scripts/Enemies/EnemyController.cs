using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private EnemyMovementBehaviour movementBehaviour;
    [SerializeField] private CharacterAttackController attackController;
    [SerializeField] private LifeManager lifeManager;

    [Header("Detection")]
    [SerializeField] private float detectionDistance = 6f;
    [SerializeField] private float loseTargetDistance = 9f;
    [Header("Patrolling")]
    [SerializeField] private bool canPatrol;
    [SerializeField] private Vector2 patrollingOffsetVector = new Vector2(1,1);
    private Vector2 startingPosition;
    private bool isGoingLeft;

    

    [Header("Attack")]
    [SerializeField] private float attackDistance = 1.5f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 1f;

    private float nextAttackTime;
    private bool isDead;
    private bool hasDetectedTarget;

    private void Awake()
    {
        if (movementBehaviour == null)
            movementBehaviour = GetComponent<EnemyMovementBehaviour>();

        if (attackController == null)
            attackController = GetComponent<CharacterAttackController>();

        if (lifeManager == null)
            lifeManager = GetComponent<LifeManager>();
        startingPosition = transform.position;
        isGoingLeft = true;
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

        if (target == null)
        {
            Stop();
            hasDetectedTarget = false;
            return;
        }

        UpdateEnemyBehaviour();
    }

    private void UpdateEnemyBehaviour()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        UpdateTargetDetection(distanceToTarget);

        if (!hasDetectedTarget)
        {
            if(canPatrol)
            {
                UpdatePatrolling();
                return;
            }
            
            
            Stop();
            return;
        }

        if (CanAttack(distanceToTarget))
        {
            TryAttack();
            return;
        }

        MoveTowardsTarget();
    }

    private void UpdatePatrolling()
    {
        if (isGoingLeft)
        {
            if (transform.position.x <= (startingPosition.x - patrollingOffsetVector.x){
                
            }
        }
        
    }

    private void UpdateTargetDetection(float distanceToTarget)
    {
        if (!hasDetectedTarget)
        {
            if (distanceToTarget <= detectionDistance)
                hasDetectedTarget = true;

            return;
        }

        if (distanceToTarget >= loseTargetDistance)
            hasDetectedTarget = false;
    }

    private bool CanAttack(float distanceToTarget)
    {
        if (Time.time < nextAttackTime)
            return false;

        if (distanceToTarget > attackDistance)
            return false;

        return true;
    }

    private void MoveTowardsTarget()
    {
        if (movementBehaviour == null)
            return;

        movementBehaviour.MoveTowards(target);
    }
    private void MoveTowardsPosition(Vector2 position)
    {
        if (movementBehaviour == null)
            return;

        movementBehaviour.MoveTowardsPosition(position);
    }

    private void Stop()
    {
        if (movementBehaviour == null)
            return;

        movementBehaviour.Stop();
    }

    private void TryAttack()
    {
        if (attackController == null)
            return;

        nextAttackTime = Time.time + attackCooldown;
        attackController.TryDefaultAttack();
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

        Stop();

        if (attackController != null)
            attackController.ClearCurrentAttack();

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