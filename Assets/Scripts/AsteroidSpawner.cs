using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;     // Prefab thiên thạch
    public float spawnInterval = 2f;      // Thời gian giữa các lần spawn
    public float xSpawnRange = 8f;        // Phạm vi spawn theo trục X
    public float ySpawnPosition = 6f;     // Vị trí spawn theo trục Y (trên màn hình)

    void Start()
    {
        InvokeRepeating("SpawnAsteroid", 1f, spawnInterval);
    }

    void SpawnAsteroid()
    {
        Vector3 spawnPos = new Vector3(Random.Range(-xSpawnRange, xSpawnRange), ySpawnPosition, 0);
        Instantiate(asteroidPrefab, spawnPos, Quaternion.identity);
    }
}
