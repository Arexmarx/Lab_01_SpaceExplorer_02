using System.Net.NetworkInformation;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public float speed = 10f;
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = AudioManager.GetInstance();
    }
    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Va chạm với: " + collision);

        if (collision.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
            audioManager.PlayExplosionSound();
        }
    }
}
