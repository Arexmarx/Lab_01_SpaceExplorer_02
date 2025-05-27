using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{

    public TextMeshProUGUI finalScoreText;


    private void Start()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        finalScoreText.text = "Score: " + score.ToString();
    }

    public void PlayAgain()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
            PlayerPrefs.DeleteKey("FinalScore");
            PlayerPrefs.Save();
        }
        if (HealthManager.instance != null)
        {
            HealthManager.instance.ResetLives();
        }
        SceneManager.LoadScene("Gameplay");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
