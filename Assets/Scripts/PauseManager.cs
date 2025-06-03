using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (isPaused) 
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        // Huỷ các manager nếu còn tồn tại
        if (ScoreManager.instance != null) Destroy(ScoreManager.instance.gameObject);
        if (HealthManager.instance != null) Destroy(HealthManager.instance.gameObject);
        if (GameplayManager.Instance != null) Destroy(GameplayManager.Instance.gameObject);

        SceneManager.LoadScene("MainMenu");
    }
}
