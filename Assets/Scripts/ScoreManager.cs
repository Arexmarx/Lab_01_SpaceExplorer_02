using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Linq;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance;
    public TMP_Text scoreText;
    int score = 0;
    int starsCollected = 0;
    public bool doubleShotEnabled = false;
    public bool tripleShotEnabled = false;
    public TMP_InputField nameInputField;


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
        scoreText.text = "Score: " + score.ToString();
    }

    //public void EndGame()
    //{
    //    SaveHighScore();
    //    PlayerPrefs.SetInt("FinalScore", score);
    //    PlayerPrefs.Save();
    //    SceneManager.LoadScene("EndGame");
    //}

    //void SaveHighScore()
    //{
    //    int currentHighScore = LoadHighScore();

    //    if (score > currentHighScore)
    //    {
    //        HighScoreData data = new HighScoreData();
    //        data.highScore = score;

    //        string json = JsonUtility.ToJson(data);
    //        File.WriteAllText(highScorePath, json);
    //    }
    //}

    public void EndGame()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        PlayerPrefs.Save();
        SceneManager.LoadScene("EndGame");
    }


    //public int LoadHighScore()
    //{
    //    if (File.Exists(highScorePath))
    //    {
    //        string json = File.ReadAllText(highScorePath);
    //        HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
    //        return data.highScore;
    //    }
    //    return 0;
    //}

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

        Debug.Log("Gọi SaveHighScoreWithName cho: " + playerName + " với điểm " + score);
        HighScoreList highScoreList;

        if (File.Exists(highScorePath))
        {
            string json = File.ReadAllText(highScorePath);
            highScoreList = JsonUtility.FromJson<HighScoreList>(json);
        }
        else
        {
            highScoreList = new HighScoreList();
        }

        HighScoreEntry newEntry = new HighScoreEntry
        {
            playerName = playerName,
            score = score
        };

        highScoreList.highScores.Add(newEntry);

        highScoreList.highScores = highScoreList.highScores
            .OrderByDescending(entry => entry.score)
            .Take(5)
            .ToList();

        string newJson = JsonUtility.ToJson(highScoreList, true);
        File.WriteAllText(highScorePath, newJson);
    }

}
