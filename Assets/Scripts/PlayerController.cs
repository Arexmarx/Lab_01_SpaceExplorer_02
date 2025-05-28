using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject laserPrefab;
    public Transform firePoint;
    private AudioManager audioManager;
    private HealthManager healthManager;
    public GameObject shieldEffect;
    private bool isShielded = false;
    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void OnEnable()
    {
        healthManager = HealthManager.instance;
        if (healthManager == null)
        {
            Debug.LogError("Không tìm thấy HealthManager trong scene!");
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");


        transform.Translate(new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime);

        ClampPositionWithinScreen();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.PlayShotSound();

            if (ScoreManager.instance.tripleShotEnabled)
            {
                // Bắn 3 tia với góc lệch trái/phải
                GameObject laserCenter = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
                GameObject laserLeft = Instantiate(laserPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 15));
                GameObject laserRight = Instantiate(laserPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -15));

                // Bỏ qua va chạm giữa 3 tia và người chơi
                Collider2D playerCollider = GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(laserCenter.GetComponent<Collider2D>(), playerCollider);
                Physics2D.IgnoreCollision(laserLeft.GetComponent<Collider2D>(), playerCollider);
                Physics2D.IgnoreCollision(laserRight.GetComponent<Collider2D>(), playerCollider);
            }
            else
            {
                GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
                Physics2D.IgnoreCollision(laser.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            }
        }
    }

    void ClampPositionWithinScreen()
    {
        Vector3 pos = transform.position;

        // Lấy biên màn hình theo world space
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Giới hạn vị trí nhân vật bên trong màn hình
        pos.x = Mathf.Clamp(pos.x, bottomLeft.x, topRight.x);
        pos.y = Mathf.Clamp(pos.y, bottomLeft.y, topRight.y);

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            if (isShielded)
            {
                DeactivateShield(); // Mất khiên nhưng không mất máu
                return;
            }

            if (healthManager != null)
            {
                healthManager.LoseLife();
                if (healthManager.IsGameOver())
                {
                    ScoreManager.instance.EndGame();
                }
            }
            else
            {
                Debug.LogError("HealthManager không tồn tại khi xử lý va chạm!");
            }
        }
    }

    public void ActivateShield()
    {
        isShielded = true;
        shieldEffect.SetActive(true);
    }

    private void DeactivateShield()
    {
        isShielded = false;
        shieldEffect.SetActive(false);
    }

}
