using UnityEngine;
using UnityEngine.Events;

public class SceneLoadTrigger : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Events")]
    public UnityEvent OnPlayerEntered;

    private bool hasTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered)
            return;

        if (!collision.transform.root.CompareTag(playerTag))
            return;

        //hasTriggered = true;
        OnPlayerEntered?.Invoke();
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    
}