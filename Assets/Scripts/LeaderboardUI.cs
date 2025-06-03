using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public TMP_Text[] rankTexts; // Gán 5 TMP_Text trong inspector

    string highScorePath;

    void Awake()
    {
        highScorePath = Path.Combine(Application.persistentDataPath, "highscore.json");
        Debug.Log("Đường dẫn file JSON: " + highScorePath);

        leaderboardPanel.SetActive(false);
    }


    public void ShowLeaderboard()
    {
        highScorePath = Path.Combine(Application.persistentDataPath, "highscore.json");
        Debug.Log("Đọc từ: " + highScorePath);

        if (!File.Exists(highScorePath))
        {
            Debug.LogWarning("Không tìm thấy file JSON!");
            return;
        }

        string json = File.ReadAllText(highScorePath);
        Debug.Log("Nội dung JSON: " + json);

        HighScoreList data = JsonUtility.FromJson<HighScoreList>(json);
        if (data == null || data.highScores == null)
        {
            Debug.LogWarning("Lỗi khi parse JSON.");
            return;
        }

        for (int i = 0; i < rankTexts.Length; i++)
        {
            if (i < data.highScores.Count)
            {
                rankTexts[i].text = $"{i + 1}. {data.highScores[i].playerName} - {data.highScores[i].score}";
            }
            else
            {
                rankTexts[i].text = $"{i + 1}. ---";
            }
        }

        leaderboardPanel.SetActive(true);
    }



    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }
}

