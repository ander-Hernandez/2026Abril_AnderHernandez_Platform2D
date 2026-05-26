using UnityEngine;
using UnityEngine.Events;

public class LifeManager : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] private float startLife = 3f;
    [SerializeField] private float currentLife;

    [Header("Events")]
    public UnityEvent<float, float> OnLifeChanged;
    public UnityEvent<float> OnLifeDepleted;

    [Header("Animation")]
    [SerializeField] private string hitTrigger = "Hit";
    [SerializeField] private Animator animator;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D rb2D;

    [Header("Debug")]
    [SerializeField] private bool debugTakeDamage;

    private bool isDead;
    [SerializeField] private CharacterMovementController2D characterMovement;

    [Header("Invunerability Settings")]
    [SerializeField] private float invulnerabilityTime;
    private float nextDamageableTime;
    private bool isDamageable;

    private void Awake()
    {
        currentLife = startLife;
        OnLifeChanged?.Invoke(currentLife, startLife);

        if (characterMovement == null)
            characterMovement = GetComponent<CharacterMovementController2D>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (rb2D == null)
            rb2D = GetComponent<Rigidbody2D>();
        isDamageable = true;
    }

    private void Update()
    {
        if (!isDamageable)
        {
            if(Time.time >= nextDamageableTime)
                {
                    isDamageable= true;
                }
            
        }
        if (debugTakeDamage)
        {
            debugTakeDamage = false;

            HitData hitData = new HitData();
            hitData.damage = 1;

            TakeHit(hitData);
        }

    }

    public void TakeHit(HitData hitData)
    {
        if (!isDamageable)
            return;
        isDamageable = false;
        nextDamageableTime = Time.time + invulnerabilityTime;

        TakeDamage(hitData.damage);
        ApplyKnockback(hitData);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (animator != null)
            animator.SetTrigger(hitTrigger);

        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0f, startLife);

        OnLifeChanged?.Invoke(currentLife, startLife);

        if (currentLife <= 0f)
        {
            Die();
        }
    }

    private void ApplyKnockback(HitData hitData)
    {
        if (characterMovement == null)
            return;

        characterMovement.ApplyKnockback(
            hitData.knockback,
            hitData.attacker,
            hitData.knockbackTime
        );
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        OnLifeDepleted?.Invoke(startLife);
    }


    public void ResetLifeManager()
    {
        isDead = false;
        currentLife = startLife;
        OnLifeChanged?.Invoke(currentLife, startLife);
    }

    public void ActivateDeadTrigger()
    {
        animator.SetTrigger("Die");
    }

    public void DieOnCustomTime(float time)
    {
        ActivateDeadTrigger();
        Destroy(gameObject, time);
    }

    public void Kill()
    {
        Die();
    }
}