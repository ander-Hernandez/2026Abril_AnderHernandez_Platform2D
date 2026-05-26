using UnityEngine;

public abstract class EnemyMovementBehaviour : MonoBehaviour
{
    public abstract void MoveTowards(Transform target);
    public abstract void MoveTowardsPosition(Vector2 target);
    public abstract void Stop();
}