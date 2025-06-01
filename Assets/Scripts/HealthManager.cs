using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;
    public Image[] hearts;          // mảng 3 Image tim
    public Sprite fullHeart;        // sprite tim đầy
    public Sprite emptyHeart;       // sprite tim rỗng

    private int maxLives = 3;
    private int currentLives;
    private bool isInitialized = false;
    private GameObject heartUICanvas; // Lưu reference đến HeartUI Canvas

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            isInitialized = true;
            
            // Tìm và lưu reference đến HeartUI ngay từ đầu
            heartUICanvas = GameObject.Find("HeartUI");
            if (heartUICanvas != null)
            {
                // Ẩn HeartUI ban đầu, sẽ được hiện khi vào scene gameplay
                heartUICanvas.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (isInitialized)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Chỉ hiện HeartUI ở các scene gameplay, ẩn ở các scene khác
        if (scene.name.StartsWith("Gameplay") && !scene.name.Contains("EndGame") && !scene.name.Contains("MainMenu"))
        {
            Debug.Log("Scene mới được load: " + scene.name + ", số mạng hiện tại: " + currentLives);
            // Hiện HeartUI và cập nhật
            if (heartUICanvas != null)
            {
                heartUICanvas.SetActive(true);
                StartCoroutine(InitializeHeartsUI());
            }
        }
        else
        {
            // Ẩn HeartUI ở MainMenu và EndGame
            if (heartUICanvas != null)
            {
                heartUICanvas.SetActive(false);
            }
        }
    }

    private System.Collections.IEnumerator InitializeHeartsUI()
    {
        yield return null; // Đợi một frame
        FindAndUpdateHeartsUI();
        UpdateHeartsUI();
    }

    void FindAndUpdateHeartsUI()
    {
        // Không tìm HeartUI ở MainMenu và EndGame
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu" || currentScene.Contains("EndGame"))
        {
            return;
        }

        if (hearts != null && hearts.Length == maxLives)
        {
            // Kiểm tra xem các tim có còn tồn tại không
            bool allHeartsValid = true;
            foreach (Image heart in hearts)
            {
                if (heart == null)
                {
                    allHeartsValid = false;
                    break;
                }
            }
            if (allHeartsValid) return; // Nếu tất cả tim đều hợp lệ thì không cần tìm lại
        }

        // Reset mảng hearts
        hearts = new Image[maxLives];
        
        // Sử dụng heartUICanvas đã lưu thay vì tìm lại
        if (heartUICanvas != null)
        {
            Canvas canvas = heartUICanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                // Tìm 3 Image tim trong Canvas
                Image[] images = canvas.GetComponentsInChildren<Image>(true);
                Debug.Log("Số lượng Image trong HeartUI Canvas: " + images.Length);

                List<Image> foundHearts = new List<Image>();

                foreach (Image img in images)
                {
                    if (img.name.Contains("Heart"))
                    {
                        Debug.Log("Tìm thấy tim: " + img.name);
                        foundHearts.Add(img);
                    }
                }

                if (foundHearts.Count >= maxLives)
                {
                    foundHearts.Sort((a, b) => a.name.CompareTo(b.name));
                    hearts = foundHearts.GetRange(0, maxLives).ToArray();
                    Debug.Log("Đã tìm thấy và sắp xếp " + hearts.Length + " tim trong HeartUI");
                    return;
                }
            }
        }

        Debug.LogError("Không tìm thấy đủ " + maxLives + " Image tim trong HeartUI Canvas!");
    }

    void Start()
    {
        if (!isInitialized) return;
        
        // Chỉ set lại số mạng nếu chưa có (lần đầu chơi)
        if (currentLives == 0)
        {
            ResetLives();
        }
        else
        {
            // Đảm bảo UI được cập nhật với số mạng hiện tại
            FindAndUpdateHeartsUI();
            UpdateHeartsUI();
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        FindAndUpdateHeartsUI();
        UpdateHeartsUI();
        Debug.Log("Reset mạng sống về " + maxLives);
    }

    public void LoseLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            Debug.Log("Mất 1 mạng, còn lại: " + currentLives);
            
            // Đảm bảo hearts đã được tìm thấy trước khi cập nhật UI
            if (hearts == null || hearts.Length == 0)
            {
                FindAndUpdateHeartsUI();
            }
            UpdateHeartsUI();
        }
    }

    void UpdateHeartsUI()
    {
        // Không cập nhật UI nếu đang ở MainMenu
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            return;
        }

        if (hearts == null || hearts.Length == 0)
        {
            Debug.LogWarning("Không thể cập nhật UI tim vì mảng hearts trống!");
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
            {
                hearts[i].sprite = (i < currentLives) ? fullHeart : emptyHeart;
                hearts[i].enabled = true; // Đảm bảo tim luôn hiển thị
                Debug.Log("Cập nhật tim " + i + ": " + (i < currentLives ? "đầy" : "rỗng") + " (số mạng: " + currentLives + ")");
            }
        }
    }

    public bool IsGameOver()
    {
        return currentLives <= 0;
    }
}
