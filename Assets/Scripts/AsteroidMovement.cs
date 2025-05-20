using UnityEngine;

public class AsteroidMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Vector2 direction;

    void Start()
    {
        // Bay từ trên xuống, có thể thêm chút ngẫu nhiên trái/phải
        float randomX = Random.Range(-0.5f, 0.5f); // bay hơi lệch trái/phải
        direction = new Vector2(randomX, -1f).normalized;
    }

    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
