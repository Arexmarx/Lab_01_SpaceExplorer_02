using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 direction;

    public GameObject purpleBubblePrefab;   // Bong bóng thường
    public GameObject speedBuffPrefab;      // Bong bóng tăng tốc

    [Range(0f, 1f)]
    public float dropChance = 0.3f; // Xác suất rơi bong bóng tăng tốc

    void Start()
    {
        float randomX = Random.Range(-0.5f, 0.5f);
        direction = new Vector2(randomX, -1f).normalized;
    }

    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public void DestroyAsteroid()
    {
        // Tỉ lệ rơi bong bóng buff (speed)
        if (speedBuffPrefab != null && Random.value <= dropChance)
        {
            Instantiate(speedBuffPrefab, transform.position, Quaternion.identity);
        }
        else if (purpleBubblePrefab != null)
        {
            // Nếu không rơi speed buff thì có thể rơi bong bóng thường
            Instantiate(purpleBubblePrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Phá hủy thiên thạch
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            DestroyAsteroid();
            Destroy(other.gameObject);
        }
    }
}
