using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections; // Thêm để sử dụng Coroutine

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject laserPrefab;
    public Transform firePoint;
    private AudioManager audioManager;
    private HealthManager healthManager;
    public GameObject shieldEffect;
    private bool isShielded = false;
    private float shieldDuration = 3f; // Thời gian tồn tại của shield
    private float shieldTimer = 0f; // Biến đếm thời gian
    private Vector3 initialPosition; // Thêm biến lưu vị trí ban đầu
    
    // Thêm các biến mới
    private SpriteRenderer spriteRenderer;
    private bool canControl = true;
    private float moveDelayDuration = 0.5f; // Thời gian chờ trước khi cho phép di chuyển lại
    [SerializeField] private float invincibilityDuration = 2f; // Thời gian tồn tại của shield sau khi hồi sinh
    private float fadeSpeed = 2f;
    private bool isInvincibleShield = false; // Thêm biến để phân biệt shield hồi sinh với shield thường
    private bool isShieldFading = false; // Thêm biến để kiểm tra shield đang fade out
    private float shieldFadeDuration = 2f; // Thời gian fade out của shield

    // Thêm property để luôn lấy lại reference mới nhất tới ScoreManager và HealthManager
    private ScoreManager scoreManager
    {
        get
        {
            if (_scoreManager == null)
                _scoreManager = FindObjectOfType<ScoreManager>();
            return _scoreManager;
        }
    }
    private ScoreManager _scoreManager;

    private HealthManager healthManagerRef
    {
        get
        {
            if (_healthManager == null)
                _healthManager = FindObjectOfType<HealthManager>();
            return _healthManager;
        }
    }
    private HealthManager _healthManager;

    private void Awake()
    {
        audioManager = AudioManager.GetInstance();
        initialPosition = transform.position; // Lưu vị trí ban đầu khi khởi tạo
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy component SpriteRenderer
        if (spriteRenderer == null)
        {
            Debug.LogError("Không tìm thấy SpriteRenderer trên Player!");
        }
    }

    private void OnEnable()
    {
        // Reset reference về null để luôn lấy lại instance mới nhất khi vào scene mới
        _scoreManager = null;
        _healthManager = null;
    }

    void Update()
    {
        if (!canControl) return; // Không cho phép điều khiển nếu đang trong thời gian chờ

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        transform.Translate(new Vector2(moveX, moveY) * moveSpeed * Time.deltaTime);

        ClampPositionWithinScreen();

        // Xử lý thời gian tồn tại của shield
        if (isShielded)
        {
            shieldTimer += Time.deltaTime;
            if (shieldTimer >= shieldDuration)
            {
                isShielded = false;
                shieldTimer = 0f;
                shieldEffect.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !PauseManager.isPaused)
        {
            // Luôn lấy lại reference trước khi bắn
            if (_scoreManager == null)
                _scoreManager = FindObjectOfType<ScoreManager>();

            audioManager.PlayShotSound();

            if (scoreManager != null && scoreManager.tripleShotEnabled)
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
            else if (scoreManager != null && scoreManager.doubleShotEnabled)
            {
                // Bắn 2 tia với góc lệch trái/phải
                GameObject laserLeft = Instantiate(laserPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, 10));
                GameObject laserRight = Instantiate(laserPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -10));

                // Bỏ qua va chạm giữa 2 tia và người chơi
                Collider2D playerCollider = GetComponent<Collider2D>();
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

    private IEnumerator FadeOutShield()
    {
        if (shieldEffect == null || isShieldFading) yield break;

        isShieldFading = true;
        SpriteRenderer shieldSprite = shieldEffect.GetComponent<SpriteRenderer>();
        if (shieldSprite == null) yield break;

        float elapsedTime = 0f;
        Color originalColor = shieldSprite.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        while (elapsedTime < shieldFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / shieldFadeDuration);
            shieldSprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        shieldEffect.SetActive(false);
        isShielded = false;
        shieldTimer = 0f;
        isShieldFading = false;
    }

    private void DeactivateShield()
    {
        // Bỏ hàm này vì không cần thiết nữa
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canControl) return; // Không xử lý va chạm nếu đang trong thời gian chờ

        if (collision.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            // Phát âm thanh nổ khi va chạm với thiên thạch (có shield hoặc không)
            audioManager.PlayExplosionSound();

            // Kiểm tra cả shield thường và shield hồi sinh
            if (isShielded || isInvincibleShield)
            {
                // Không làm gì cả, shield vẫn tiếp tục tồn tại cho đến hết thời gian
                return;
            }

            if (healthManagerRef != null)
            {
                healthManagerRef.LoseLife();
                StartCoroutine(ResetPlayerWithEffect());
                if (healthManagerRef.IsGameOver())
                {
                    if (scoreManager != null)
                        scoreManager.EndGame();
                }
            }
            else
            {
                Debug.LogError("HealthManager không tồn tại khi xử lý va chạm!");
            }
        }
    }

    private IEnumerator ResetPlayerWithEffect()
    {
        // Tắt điều khiển
        canControl = false;

        // Hiệu ứng fade out
        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.Max(0, alpha);
            spriteRenderer.color = color;
            yield return null;
        }

        // Reset vị trí
        transform.position = initialPosition;
        if (isShielded)
        {
            DeactivateShield();
        }

        // Kích hoạt shield hồi sinh
        isInvincibleShield = true;
        shieldEffect.SetActive(true);

        // Đợi một chút trước khi fade in
        yield return new WaitForSeconds(0.2f);

        // Hiệu ứng fade in
        alpha = 0f;
        while (alpha < 1)
        {
            alpha += fadeSpeed * Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.Min(1, alpha);
            spriteRenderer.color = color;
            yield return null;
        }

        // Đợi thời gian cho phép di chuyển lại
        yield return new WaitForSeconds(moveDelayDuration);
        canControl = true; // Cho phép di chuyển lại sau 0.5 giây

        // Đợi thêm thời gian invincibility còn lại
        yield return new WaitForSeconds(invincibilityDuration - moveDelayDuration);

        // Tắt shield hồi sinh
        isInvincibleShield = false;
        shieldEffect.SetActive(false);
    }

    public void ActivateShield()
    {
        if (!isInvincibleShield) // Chỉ cho phép kích hoạt shield thường nếu không đang trong shield hồi sinh
        {
            isShielded = true;
            shieldTimer = 0f;
            shieldEffect.SetActive(true);
        }
    }

}
