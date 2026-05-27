using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    private GameObject currentCamera;
    public void TeleportPlayer(Transform player)
    {
        if (player == null)
            return;

        if (targetTransform == null)
            return;

        player.position = targetTransform.position;
    }
}