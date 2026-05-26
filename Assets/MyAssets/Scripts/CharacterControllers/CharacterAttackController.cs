using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    [SerializeField] private AttackBase defaultAttack;

    private AttackBase currentAttack;

    public bool IsAttackExecuting
    {
        get
        {
            if (currentAttack == null)
                return false;

            return currentAttack.IsExecuting;
        }
    }

    public void TryDefaultAttack()
    {
        TryAttack(defaultAttack);
    }

    public void TryAttack(AttackBase attack)
    {
        if (attack == null)
            return;

        currentAttack = attack;
        attack.TryExecute();
    }

    public void ClearCurrentAttack()
    {
        if (currentAttack != null)
            currentAttack.ClearAttack();

        if (defaultAttack != null && defaultAttack != currentAttack)
            defaultAttack.ClearAttack();

        currentAttack = null;
    }
}