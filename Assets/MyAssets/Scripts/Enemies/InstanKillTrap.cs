using UnityEngine;

public class InstanKillTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        LifeManager lifeManager = collision.GetComponentInParent<LifeManager>();

        if (lifeManager == null)
            return;

        lifeManager.Kill();
    }
}
