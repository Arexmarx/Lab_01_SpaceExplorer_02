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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Hủy đăng ký sự kiện khi object bị hủy
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Gameplay"))
        {
            Debug.Log("Scene mới được load: " + scene.name + ", số mạng hiện tại: " + currentLives);
            // Reset hearts array để tìm lại UI trong scene mới
            hearts = null;
            FindAndUpdateHeartsUI();
            // Cập nhật UI theo số mạng hiện tại
            UpdateHeartsUI();
        }
    }

    void FindAndUpdateHeartsUI()
    {
        // Reset mảng hearts trước khi tìm mới
        hearts = null;
        
        // Tìm Canvas trong scene mới
        Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log("Số lượng Canvas tìm thấy: " + canvases.Length);

        foreach (Canvas canvas in canvases)
        {
            // Tìm 3 Image tim trong Canvas
            Image[] images = canvas.GetComponentsInChildren<Image>();
            Debug.Log("Số lượng Image trong Canvas: " + images.Length);

            List<Image> foundHearts = new List<Image>();

            foreach (Image img in images)
            {
                if (img.name.Contains("Heart"))  // Tìm các tim theo tên
                {
                    Debug.Log("Tìm thấy tim: " + img.name);
                    foundHearts.Add(img);
                }
            }

            if (foundHearts.Count >= maxLives) // tìm đủ 3 tim
            {
                hearts = foundHearts.ToArray();
                Debug.Log("Đã tìm thấy đủ " + hearts.Length + " tim");
                return;
            }
        }

        Debug.LogError("Không tìm thấy đủ " + maxLives + " Image tim trong Canvas mới!");
    }

    void Start()
    {
        // Chỉ set lại số mạng nếu chưa có (lần đầu chơi)
        if (currentLives == 0)
        {
            ResetLives();
        }
    }

    public void ResetLives()
    {
        currentLives = maxLives;
        // Reset hearts array để tìm lại UI trong scene mới
        hearts = null;
        // Tìm và cập nhật UI ngay lập tức
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

            if (currentLives <= 0)
            {
                if (ScoreManager.instance != null)
                {
                    ScoreManager.instance.EndGame();
                }
            }
        }
    }

    void UpdateHeartsUI()
    {
        if (hearts != null && hearts.Length > 0)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null)
                {
                    hearts[i].sprite = (i < currentLives) ? fullHeart : emptyHeart;
                    Debug.Log("Cập nhật tim " + i + ": " + (i < currentLives ? "đầy" : "rỗng") + " (số mạng: " + currentLives + ")");
                }
            }
        }
        else
        {
            Debug.LogWarning("Không thể cập nhật UI tim vì mảng hearts trống!");
        }
    }

    public bool IsGameOver()
    {
        return currentLives <= 0;
    }
}
