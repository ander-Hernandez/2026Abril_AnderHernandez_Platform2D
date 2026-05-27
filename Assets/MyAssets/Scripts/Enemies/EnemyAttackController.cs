using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected CharacterAttackController characterAttackController;

    [Header("Attack")]
    [SerializeField] protected float attackDistance = 1.5f;
    [SerializeField] protected float attackCooldown = 1f;

    protected float nextAttackTime;
    protected Transform currentTarget;

    public bool IsAttackExecuting
    {
        get
        {
            if (characterAttackController == null)
                return false;

            return characterAttackController.IsAttackExecuting;
        }
    }

    protected virtual void Awake()
    {
        if (characterAttackController == null)
            characterAttackController = GetComponent<CharacterAttackController>();
    }

    public virtual bool CanAttack(Transform target)
    {
        if (target == null)
            return false;

        if (!IsCooldownReady())
            return false;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > attackDistance)
            return false;

        return true;
    }

    protected bool IsCooldownReady()
    {
        return Time.time >= nextAttackTime;
    }

    public virtual void TryAttack()
    {
        if (characterAttackController == null)
            return;

        nextAttackTime = Time.time + attackCooldown;
        characterAttackController.TryDefaultAttack();
    }

    public virtual void TryAttack(Transform target)
    {
        currentTarget = target;
        TryAttack();
    }

    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }

    public virtual void ClearAttack()
    {
        currentTarget = null;

        if (characterAttackController == null)
            return;

        characterAttackController.ClearCurrentAttack();
    }
}