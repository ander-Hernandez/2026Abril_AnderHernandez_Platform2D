using System.Collections;
using UnityEngine;

public class SpawnTrainingDummy : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool loopEnemies = true;

    private int currentEnemyIndex;
    private GameObject currentEnemy;
    private LifeManager currentEnemyLifeManager;
    private Coroutine spawnCoroutine;

    private void Start()
    {
        if (spawnOnStart)
            SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        if (currentEnemyIndex >= enemyPrefabs.Length)
        {
            if (!loopEnemies)
                return;

            currentEnemyIndex = 0;
        }

        GameObject prefabToSpawn = enemyPrefabs[currentEnemyIndex];

        if (prefabToSpawn == null)
            return;

        Vector3 spawnPosition = transform.position;

        if (spawnPoint != null)
            spawnPosition = spawnPoint.position;

        currentEnemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        AssignTarget(currentEnemy);
        SubscribeToEnemyDeath(currentEnemy);

        currentEnemyIndex++;
    }

    private void AssignTarget(GameObject enemy)
    {
        if (enemy == null)
            return;

        EnemyController enemyController = enemy.GetComponent<EnemyController>();

        if (enemyController == null)
            return;

        enemyController.SetTarget(target);
    }

    private void SubscribeToEnemyDeath(GameObject enemy)
    {
        if (enemy == null)
            return;

        currentEnemyLifeManager = enemy.GetComponent<LifeManager>();

        if (currentEnemyLifeManager == null)
            return;

        currentEnemyLifeManager.OnLifeDepleted.AddListener(OnCurrentEnemyDead);
    }

    private void UnsubscribeFromEnemyDeath()
    {
        if (currentEnemyLifeManager == null)
            return;

        currentEnemyLifeManager.OnLifeDepleted.RemoveListener(OnCurrentEnemyDead);
        currentEnemyLifeManager = null;
    }

    private void OnCurrentEnemyDead(float startLife)
    {
        UnsubscribeFromEnemyDeath();

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        SpawnNextEnemy();

        spawnCoroutine = null;
    }
}
