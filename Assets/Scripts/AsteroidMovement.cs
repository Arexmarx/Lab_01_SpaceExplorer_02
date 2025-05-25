using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 direction;

    public GameObject purpleBubblePrefab;

    [Range(0f, 1f)] // Cho phép chỉnh slider dễ dàng trong Inspector, giới hạn từ 0 đến 1
    public float dropChance = 0.3f; // Tỉ lệ rơi bong bóng (0.0 - 1.0)

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
        if (purpleBubblePrefab != null)
        {
            if (Random.value <= dropChance)
            {
                Instantiate(purpleBubblePrefab, transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
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
