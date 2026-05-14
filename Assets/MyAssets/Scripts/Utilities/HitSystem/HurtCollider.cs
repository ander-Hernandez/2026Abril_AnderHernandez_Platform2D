using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent<HitData> onHitEvent;

    internal void NotifyHit(HitData hitData)
    {
        onHitEvent.Invoke(hitData);
    }
}