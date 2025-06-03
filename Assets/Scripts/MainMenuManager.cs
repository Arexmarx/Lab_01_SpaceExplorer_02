using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;
    public TMP_Text highScoreText;
    string highScorePath;
    private MenuAudioManager menuAudio;

    private void Start()
    {
        highScorePath = Application.persistentDataPath + "/highscore.json";
        LoadAndShowHighScore();
        menuAudio = GetComponent<MenuAudioManager>();
    }

    void LoadAndShowHighScore()
    {
        int highScore = 0;

        if (File.Exists(highScorePath))
        {
            string json = File.ReadAllText(highScorePath);
            HighScoreList data = JsonUtility.FromJson<HighScoreList>(json);

            if (data != null && data.highScores != null && data.highScores.Count > 0)
            {
                highScore = data.highScores[0].score; // Vì danh sách đã được sắp theo thứ tự giảm dần
            }
        }

        highScoreText.text = "Highest Score: " + highScore.ToString();
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
