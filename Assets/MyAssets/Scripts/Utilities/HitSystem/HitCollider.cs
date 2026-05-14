using System;
using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour
{
    [SerializeField] private HitData hitData;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HurtCollider hurtCollider = collision.GetComponent<HurtCollider>();

        if (hurtCollider != null)
        {

            GameObject attacker = transform.root.gameObject;

            if (hurtCollider.transform.root.gameObject == attacker)
                return;



            HitData data = hitData;
            data.attacker = attacker;

            hurtCollider.NotifyHit(data);
        }
    }
}
