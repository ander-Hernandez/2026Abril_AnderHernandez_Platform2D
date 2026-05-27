using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CheckpointBehaviour : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] private bool triggerOnlyOnce = true;

    [Header("Scene Load")]
    [SerializeField] private string sceneName;

    [Header("Events")]
    public UnityEvent OnCheckpointActivated;

    private bool hasBeenActivated;

    private void Awake()
    {
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggerOnlyOnce && hasBeenActivated)
            return;

        if (!collision.transform.root.CompareTag(playerTag))
            return;

        PlayerControl playerControl = collision.GetComponentInParent<PlayerControl>();

        if (playerControl == null)
            playerControl = collision.transform.root.GetComponent<PlayerControl>();

        if (playerControl == null)
            return;

        playerControl.SetRespawnPoint(spawnPoint);

        hasBeenActivated = true;

        OnCheckpointActivated?.Invoke();
    }

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("CheckpointBehaviour: Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(string sceneToLoad)
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("CheckpointBehaviour: Scene name is empty.");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void ResetCheckpoint()
    {
        hasBeenActivated = false;
    }
}