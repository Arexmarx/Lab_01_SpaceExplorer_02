using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject laserPrefab;
    public Transform firePoint;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
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
            GameObject laser = Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
            Physics2D.IgnoreCollision(laser.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
            ScoreManager.instance.EndGame();
        }
    }

}
