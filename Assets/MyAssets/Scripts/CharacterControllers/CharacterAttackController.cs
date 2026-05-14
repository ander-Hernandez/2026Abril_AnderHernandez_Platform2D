using UnityEngine;

public class CharacterAttackController : MonoBehaviour
{
    [SerializeField] private AttackBase defaultAttack;

    public void TryDefaultAttack()
    {
        TryAttack(defaultAttack);
    }

    public void TryAttack(AttackBase attack)
    {
        if (attack == null)
            return;

        attack.TryExecute();
    }
}