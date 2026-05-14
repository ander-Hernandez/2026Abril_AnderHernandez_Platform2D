using System.Collections;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public bool IsAttacking { get; private set; }

    private void Awake()
    {
        IsAttacking = false;
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void PlayAttack(string triggerName)
    {
        if (IsAttacking)
        {
            Debug.Log($"{name}: Cannot attack, already attacking.");
            return;
        }

        if (animator == null)
        {
            Debug.LogWarning($"{name}: No Animator assigned.");
            return;
        }

        IsAttacking = true;

        Debug.Log($"{name}: Playing attack animation: {triggerName}");
        animator.SetTrigger(triggerName);
    }

    public void EnableHitBox(GameObject hitBox, float activeTime)
    {
        if (hitBox == null)
        {
            Debug.LogWarning($"{name}: Tried to enable a null hitbox.");
            return;
        }

        StartCoroutine(TemporaryEnableHitBox(hitBox, activeTime));
    }

    private IEnumerator TemporaryEnableHitBox(GameObject hitBox, float activeTime)
    {
        Debug.Log($"{name}: Hitbox enabled: {hitBox.name}");

        hitBox.SetActive(true);

        yield return new WaitForSeconds(activeTime);

        hitBox.SetActive(false);

        Debug.Log($"{name}: Hitbox disabled: {hitBox.name}");
    }

    public void OnAttackFinished()
    {
        Debug.Log($"{name}: Attack finished.");
        IsAttacking = false;
    }
}