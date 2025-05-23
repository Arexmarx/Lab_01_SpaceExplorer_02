using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    public TMP_Text scoreText;
    int score = 0;
    int starsCollected = 0;
    public bool tripleShotEnabled = false;

    string highScorePath;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Không huỷ khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        string folderPath;

        //#if UNITY_EDITOR
        //        folderPath = Application.dataPath + "/Data";
        //#else
            folderPath = Application.persistentDataPath;
        //#endif

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        highScorePath = folderPath + "/highscore.json";
        Debug.Log("Lưu tại: " + Application.persistentDataPath);
    }


    public void AddScore(int value)
    {
        score += value;
        starsCollected++;

        if (starsCollected >= 10 && !tripleShotEnabled)
        {
            tripleShotEnabled = true;
            Debug.Log("Đã mở khóa Triple Shot!");
        }

        UpdateScoreUI();
        if (score == 70 )
        {
            SceneManager.LoadScene("Gameplay_2");
        } else if (score == 100) 
        {
            SceneManager.LoadScene("Gameplay_3");
        }

    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void EndGame()
    {
        SaveHighScore();
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EndGame");
    }

    void SaveHighScore()
    {
        int currentHighScore = LoadHighScore();

        if (score > currentHighScore)
        {
            HighScoreData data = new HighScoreData();
            data.highScore = score;

            string json = JsonUtility.ToJson(data);
            File.WriteAllText(highScorePath, json);
        }
    }

    public int LoadHighScore()
    {
        if (File.Exists(highScorePath))
        {
            string json = File.ReadAllText(highScorePath);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            return data.highScore;
        }
        return 0;
    }
}
