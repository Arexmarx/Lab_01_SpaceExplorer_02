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
        }
        SceneManager.LoadScene("Gameplay");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
