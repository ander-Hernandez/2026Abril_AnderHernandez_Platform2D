using UnityEngine;

public abstract class EnemyMovementController : MonoBehaviour
{
    public abstract void UpdateIdleMovement();

    public abstract void MoveTowards(Transform target);

    public abstract void Stop();
}