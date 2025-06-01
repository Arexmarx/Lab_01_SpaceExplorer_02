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
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);
            highScore = data.highScore;
        }

        highScoreText.text = "High Score: " + highScore.ToString();
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
