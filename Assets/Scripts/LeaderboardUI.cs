using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject leaderboardPanel;       
    public TMP_Text[] rankTexts;              
    private void Awake()
    {
        leaderboardPanel.SetActive(false);    
    }

    public void ShowLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        
        StartCoroutine(GetLeaderboardFromApi());
    }


    IEnumerator GetLeaderboardFromApi()
    {
        string url = "https://todo-app-be-6p6d.onrender.com/api/highscores/top";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            HighScoreEntryResponseList result = JsonUtility.FromJson<HighScoreEntryResponseList>("{\"scores\":" + json + "}");

            if (result != null && result.scores != null)
            {
                for (int i = 0; i < rankTexts.Length; i++)
                {
                    if (i < result.scores.Length)
                    {
                        rankTexts[i].text = $"{i + 1}. {result.scores[i].playerName} - {result.scores[i].score}";
                    }
                    else
                    {
                        rankTexts[i].text = $"{i + 1}. ---";
                    }
                }
            }
            else
            {
                Debug.LogWarning("Không có dữ liệu điểm cao từ API.");
                ClearLeaderboardUI();
            }
        }
        else
        {
            Debug.LogError("Lỗi khi lấy leaderboard từ API: " + request.error);
            ClearLeaderboardUI();
        }
    }

    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    private void ClearLeaderboardUI()
    {
        for (int i = 0; i < rankTexts.Length; i++)
        {
            rankTexts[i].text = $"{i + 1}. ---";
        }
    }
}
