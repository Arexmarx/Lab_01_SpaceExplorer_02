using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Tooltip("Tốc độ di chuyển cơ bản của người chơi.")]
    private float moveSpeed = 2f;

    [Header("Laser Settings")]
    [SerializeField, Tooltip("Prefab của tia laser để bắn.")]
    private GameObject laserPrefab;
    [SerializeField, Tooltip("Điểm bắn laser trên người chơi.")]
    private Transform firePoint;

    [Header("Shield Settings")]
    [SerializeField, Tooltip("Hiệu ứng lá chắn khi người chơi kích hoạt.")]
    private GameObject shieldEffect;
    [SerializeField, Tooltip("Thời gian lá chắn tồn tại.")]
    private float shieldDuration = 3f;
    [SerializeField, Tooltip("Thời gian hiệu ứng lá chắn mờ dần.")]
    private float shieldFadeDuration = 2f;
    [SerializeField, Tooltip("Thời gian bất tử sau khi hết lá chắn.")]
    private float invincibilityDuration = 2f;

    [Header("Fade Settings")]
    [SerializeField, Tooltip("Tốc độ fade in/out khi chết hoặc hồi sinh.")]
    private float fadeSpeed = 2f;
    [SerializeField, Tooltip("Thời gian delay trước khi người chơi được điều khiển lại.")]
    private float moveDelayDuration = 0.5f;

    private float currentSpeed;
    private bool isBuffed = false;

    private AudioManager audioManager;
    private SpriteRenderer spriteRenderer;

    private bool canControl = true;
    private bool isShielded = false;
    private bool isInvincibleShield = false;
    private bool isShieldFading = false;

    private float shieldTimer = 0f;
    private Vector3 initialPosition;

    private ScoreManager _scoreManager;
    private HealthManager _healthManager;

    private ScoreManager scoreManager
    {
        get
        {
            if (_scoreManager == null)
                _scoreManager = FindObjectOfType<ScoreManager>();
            return _scoreManager;
        }
    }

    private HealthManager healthManagerRef
    {
        get
        {
            if (_healthManager == null)
                _healthManager = FindObjectOfType<HealthManager>();
            return _healthManager;
        }
    }

    private void Awake()
    {
        audioManager = AudioManager.GetInstance();
        initialPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentSpeed = moveSpeed;

        if (spriteRenderer == null)
        {
            Debug.LogError("Không tìm thấy SpriteRenderer trên Player!");
        }
    }

    private void OnEnable()
    {
        _scoreManager = null;
        _healthManager = null;
    }

    private void Update()
    {
        if (!canControl) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        transform.Translate(new Vector2(moveX, moveY) * currentSpeed * Time.deltaTime);

        ClampPositionWithinScreen();

        if (isShielded)
        {
            shieldTimer += Time.deltaTime;
            if (shieldTimer >= shieldDuration)
            {
                StartCoroutine(FadeOutShield());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !PauseManager.isPaused)
        {
            audioManager.PlayShotSound();

            Collider2D playerCollider = GetComponent<Collider2D>();

            if (scoreManager != null && scoreManager.tripleShotEnabled)
            {
                FireLaser(0);
                FireLaser(15);
                FireLaser(-15);
            }
            else if (scoreManager != null && scoreManager.doubleShotEnabled)
            {
                FireLaser(10);
                FireLaser(-10);
            }
            else
            {
                GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
                Physics2D.IgnoreCollision(laser.GetComponent<Collider2D>(), playerCollider);
            }
        }
    }

    private void FireLaser(float angleOffset)
    {
        Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, angleOffset);
        GameObject laser = Instantiate(laserPrefab, firePoint.position, rotation);
        Physics2D.IgnoreCollision(laser.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    private void ClampPositionWithinScreen()
    {
        Vector3 pos = transform.position;
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canControl) return;

        if (collision.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            audioManager.PlayExplosionSound();

            if (isShielded || isInvincibleShield) return;

            if (healthManagerRef != null)
            {
                healthManagerRef.LoseLife();
                StartCoroutine(ResetPlayerWithEffect());

                if (healthManagerRef.IsGameOver())
                {
                    scoreManager?.EndGame();
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
        canControl = false;

        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.Max(0, alpha);
            spriteRenderer.color = color;
            yield return null;
        }

        transform.position = initialPosition;

        isInvincibleShield = true;
        shieldEffect.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        alpha = 0f;
        while (alpha < 1)
        {
            alpha += fadeSpeed * Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.Min(1, alpha);
            spriteRenderer.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(moveDelayDuration);
        canControl = true;

        yield return new WaitForSeconds(invincibilityDuration - moveDelayDuration);
        isInvincibleShield = false;
        shieldEffect.SetActive(false);
    }

    public void ActivateShield()
    {
        if (!isInvincibleShield)
        {
            isShielded = true;
            shieldTimer = 0f;
            shieldEffect.SetActive(true);
        }
    }

    public void ApplySpeedBuff(float multiplier, float duration)
    {
        if (!isBuffed)
        {
            StartCoroutine(SpeedBuffRoutine(multiplier, duration));
        }
    }

    private IEnumerator SpeedBuffRoutine(float multiplier, float duration)
    {
        isBuffed = true;
        currentSpeed = moveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        currentSpeed = moveSpeed;
        isBuffed = false;
    }
}
