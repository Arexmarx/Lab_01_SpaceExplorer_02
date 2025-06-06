using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;
    public TMP_InputField nameInputField;

    int score = 0;
    int starsCollected = 0;
    public bool doubleShotEnabled = false;
    public bool tripleShotEnabled = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Debug.Log("Lưu tại: " + Application.persistentDataPath);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
        starsCollected++;

        if (starsCollected >= 13 && !doubleShotEnabled)
        {
            doubleShotEnabled = true;
            Debug.Log("Đã mở khóa Double Shot!");
        }

        if (starsCollected >= 25 && !tripleShotEnabled)
        {
            tripleShotEnabled = true;
            Debug.Log("Đã mở khóa Triple Shot!");
        }

        UpdateScoreUI();

        if (starsCollected == 10)
        {
            SceneManager.LoadScene("Gameplay_2");
        }
        else if (starsCollected == 20)
        {
            SceneManager.LoadScene("Gameplay_3");
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void EndGame()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EndGame");
    }

    public void ResetScore()
    {
        score = 0;
        starsCollected = 0;
        doubleShotEnabled = false;
        tripleShotEnabled = false;
        UpdateScoreUI();
    }

    public void SaveHighScoreWithName(string playerName)
    {
        Debug.Log($"Gửi điểm số {score} của {playerName} lên API...");
        StartCoroutine(SendHighScoreToApi(playerName, score));
    }

    IEnumerator SendHighScoreToApi(string playerName, int score)
    {
        HighScoreEntry entry = new HighScoreEntry
        {
            playerName = playerName,
            score = score
        };

        string json = JsonUtility.ToJson(entry);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest("https://todo-app-be-6p6d.onrender.com/api/highscores", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

#if UNITY_EDITOR
            request.certificateHandler = new BypassCertificate(); 
#endif

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ High score sent successfully!");
                Debug.Log("Phản hồi từ server: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Error sending high score: " + request.error);
            }
        }
    }

#if UNITY_EDITOR
    class BypassCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
#endif
}
