using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    public float speedMultiplier = 12f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplySpeedBuff(speedMultiplier, duration);
                Destroy(gameObject); // Tự hủy sau khi dùng xong
            }
        }
    }
}
