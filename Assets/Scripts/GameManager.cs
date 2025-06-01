using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject healthManagerPrefab;
    [SerializeField] private GameObject scoreManagerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        // Khởi tạo AudioManager nếu chưa tồn tại
        if (AudioManager.Instance == null)
        {
            if (audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }
            else
            {
                Debug.LogError("AudioManager Prefab chưa được gán trong GameManager!");
            }
        }

        // Khởi tạo HealthManager nếu chưa tồn tại
        if (HealthManager.instance == null)
        {
            if (healthManagerPrefab != null)
            {
                Instantiate(healthManagerPrefab);
            }
            else
            {
                Debug.LogError("HealthManager Prefab chưa được gán trong GameManager!");
            }
        }

        // Khởi tạo ScoreManager nếu chưa tồn tại
        if (ScoreManager.instance == null)
        {
            if (scoreManagerPrefab != null)
            {
                Instantiate(scoreManagerPrefab);
            }
            else
            {
                Debug.LogError("ScoreManager Prefab chưa được gán trong GameManager!");
            }
        }
    }

    public void ResetGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResetAudio();
        }
        if (HealthManager.instance != null)
        {
            HealthManager.instance.ResetLives();
        }
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
    }
}