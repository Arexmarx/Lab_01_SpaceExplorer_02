using UnityEngine;
using UnityEngine.SceneManagement;

public class Star : MonoBehaviour
{
    public float moveSpeed = 2f;
    private AudioManager audioManager;


    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    void Update()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {   
            ScoreManager.instance.AddScore(10);
            Destroy(gameObject);
            audioManager.PlayStarSound();
        }
    }
}
