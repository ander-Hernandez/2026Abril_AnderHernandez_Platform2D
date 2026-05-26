using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private bool shouldSpawn = true;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float timeToSpawn;
    [SerializeField] private Transform player;

    private float nextSpawnTime;

    private void Awake()
    {
        nextSpawnTime = Time.time + timeToSpawn;
    }

    private void Update()
    {
        if (!shouldSpawn)
            return;

        if (Time.time < nextSpawnTime)
            return;

        SpawnEnemy();
        nextSpawnTime = Time.time + timeToSpawn;
    }

    private void SpawnEnemy()
    {
        GameObject instance = Instantiate(enemyPrefab, transform.position, transform.rotation);

        EnemyController enemyController = instance.GetComponent<EnemyController>();

        if (enemyController != null)
            enemyController.SetTarget(player);
    }
}