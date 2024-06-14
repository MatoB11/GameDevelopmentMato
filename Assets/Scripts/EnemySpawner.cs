using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Referencia na prefabrikát nepriateľa, ktorý sa má spawnovať
    public GameObject enemyPrefab;
    // GameObject, na ktorom sa má nepriateľ spawnovať
    public GameObject spawnLocation;
    // Interval medzi spawnovaním nepriateľov (v sekundách)
    public float spawnInterval = 5f;

    void Start()
    {
        // Spustíme metódu SpawnEnemy() po prvom spawnovaní nepriateľa
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Skontroluje, či sú priradené potrebné objekty
        if (enemyPrefab != null && spawnLocation != null)
        {
            // Spawnuje nepriateľa na pozícii a rotácii spawnLocation
            Instantiate(enemyPrefab, spawnLocation.transform.position, spawnLocation.transform.rotation);
        }
        else
        {
            Debug.LogError("EnemyPrefab alebo SpawnLocation nie je priradené v Inspector.");
        }
    }
}
