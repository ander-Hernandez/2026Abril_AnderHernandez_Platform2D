using UnityEngine;

[System.Serializable]
public struct HitData
{
    public int damage;
    public float hitPauseTime;
    public Vector2 knockback;
    public float knockbackTime;
    public GameObject attacker;
}