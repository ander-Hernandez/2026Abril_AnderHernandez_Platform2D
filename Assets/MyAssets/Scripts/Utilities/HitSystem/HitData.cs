using UnityEngine;

[System.Serializable]
public struct HitData
{
    public int damage;
    public Vector2 knockback;
    public GameObject attacker;
}