using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject instructionsPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
