using UnityEngine;

public class BananaCoinBehaviour : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] private int pointsToAdd = 1;

    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool wasCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wasCollected)
            return;

        if (!collision.transform.root.CompareTag(playerTag))
            return;

        Collect();
    }

    private void Collect()
    {
        wasCollected = true;

        if (PointManager.Instance != null)
        {
            PointManager.Instance.AddPoints(pointsToAdd);
            AudioManager.Instance.PlayCoinSound();
        }
        else
        {
            Debug.LogWarning("BananaCoinBehaviour: No PointManager found in scene.");
        }

        Destroy(gameObject);
    }
}