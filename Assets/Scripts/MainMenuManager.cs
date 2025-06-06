using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;
    public TMP_Text highScoreText;
    private MenuAudioManager menuAudio;

    private void Start()
    {
        menuAudio = GetComponent<MenuAudioManager>();
        StartCoroutine(LoadAndShowHighScoreFromApi());
    }

    IEnumerator LoadAndShowHighScoreFromApi()
    {
        string url = "https://todo-app-be-6p6d.onrender.com/api/highscores/top";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            HighScoreEntryResponseList result = JsonUtility.FromJson<HighScoreEntryResponseList>("{\"scores\":" + json + "}");

            if (result != null && result.scores != null && result.scores.Length > 0)
            {
                highScoreText.text = "Highest Score: " + result.scores[0].score;
            }
            else
            {
                highScoreText.text = "Highest Score: 0";
            }
        }
        else
        {
            Debug.LogError("Failed to fetch high score: " + request.error);
            highScoreText.text = "Highest Score: 0";
        }
    }

    public void PlayGame()
    {
        if (menuAudio != null)
        {
            menuAudio.StopMenuBackgroundMusic();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.ResetGame();
        }

        SceneManager.LoadScene("Gameplay");
    }

    public void ToggleInstruction()
    {
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class HighScoreEntryResponse
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class HighScoreEntryResponseList
{
    public HighScoreEntryResponse[] scores;
}
