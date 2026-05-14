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

    [Header("Debug")]
    [SerializeField] private bool debugTakeDamage;

    private bool isDead;

    private void Awake()
    {
        currentLife = startLife;
        OnLifeChanged?.Invoke(currentLife, startLife);
    }

    private void Update()
    {
        if (debugTakeDamage)
        {
            debugTakeDamage = false;
            TakeDamage(1f);
        }
    }

    public void TakeHit(HitData hitData)
    {
        TakeDamage(hitData.damage);
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        Debug.Log("LifeManager: TakingDamage");

        currentLife -= damage;
        currentLife = Mathf.Clamp(currentLife, 0f, startLife);

        OnLifeChanged?.Invoke(currentLife, startLife);

        if (currentLife <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        OnLifeDepleted?.Invoke(startLife);
    }
}