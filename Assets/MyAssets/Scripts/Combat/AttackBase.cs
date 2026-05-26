using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public virtual bool IsExecuting
    {
        get { return false; }
    }

    public abstract void TryExecute();

    public abstract void ClearAttack();
}