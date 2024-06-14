using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int count;
        public float rate;
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class WaveSet
    {
        public Wave[] waves;
        public float timeBetweenWaves;
    }

    public WaveSet[] waveSets;
    public TMP_Text waveText;
    public Button nextWaveButton;

    private int currentWaveSetIndex = 0;
    private int currentWaveIndex = 0;
    private int aliveEnemies = 0;
    private bool isWaveActive = false;

    void Start()
    {
        if (nextWaveButton != null)
        {
            nextWaveButton.gameObject.SetActive(false);
            nextWaveButton.onClick.AddListener(OnNextWaveButtonClicked);
        }
        else
        {
            Debug.LogError("NextWaveButton is not assigned in the inspector!");
        }
        StartCoroutine(SpawnWaveSet());
    }

    void Update()
    {
        if (!isWaveActive && aliveEnemies <= 0 && currentWaveIndex >= waveSets[currentWaveSetIndex].waves.Length)
        {
            if (currentWaveSetIndex < waveSets.Length - 1)
            {
                nextWaveButton.gameObject.SetActive(true);
                Debug.Log("Wave completed. Waiting for next wave to start.");
            }
            else
            {
                Debug.Log("All wave sets completed.");
            }
        }
    }

    IEnumerator SpawnWaveSet()
    {
        isWaveActive = true;

        if (currentWaveSetIndex < waveSets.Length)
        {
            Debug.Log("Spawning wave set " + currentWaveSetIndex);
            waveText.text = "Wave " + (currentWaveSetIndex + 1);
            yield return new WaitForSeconds(1f);

            for (currentWaveIndex = 0; currentWaveIndex < waveSets[currentWaveSetIndex].waves.Length; currentWaveIndex++)
            {
                Debug.Log("Starting wave " + (currentWaveSetIndex + 1) + ", part " + (currentWaveIndex + 1));
                yield return StartCoroutine(SpawnWave(waveSets[currentWaveSetIndex].waves[currentWaveIndex]));
                yield return new WaitForSeconds(waveSets[currentWaveSetIndex].timeBetweenWaves);
            }

            isWaveActive = false;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        aliveEnemies += wave.count;
        Debug.Log("Spawning " + wave.count + " enemies in wave " + (currentWaveSetIndex + 1) + ", part " + (currentWaveIndex + 1));

        for (int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemyPrefab, wave.spawnPoint);
            yield return new WaitForSeconds(1f / wave.rate);
        }
    }

    void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += OnEnemyDeath;
        }
        else
        {
            Debug.LogError("EnemyHealth component missing on enemy prefab.");
        }
    }

    void OnEnemyDeath()
    {
        aliveEnemies--;
        Debug.Log("Enemy died. Alive enemies: " + aliveEnemies);

        if (aliveEnemies < 0)
        {
            aliveEnemies = 0;
        }

        // Check if wave is completed
        if (aliveEnemies == 0 && !isWaveActive)
        {
            Debug.Log("Wave set " + (currentWaveSetIndex + 1) + " completed.");
        }
    }

    public void OnNextWaveButtonClicked()
    {
        nextWaveButton.gameObject.SetActive(false);

        if (currentWaveSetIndex < waveSets.Length - 1)
        {
            currentWaveSetIndex++;
            currentWaveIndex = 0;
            Debug.Log("Proceeding to wave set " + (currentWaveSetIndex + 1));
            StartCoroutine(SpawnWaveSet());
        }
        else
        {
            Debug.Log("All wave sets completed.");
        }
    }
}
