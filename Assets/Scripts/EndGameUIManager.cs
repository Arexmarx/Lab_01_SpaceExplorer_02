using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameUIManager : MonoBehaviour
{
    public GameObject nameInputPanel;       // Panel chứa form nhập tên
    public TMP_InputField nameInputField;   // InputField để nhập tên
    public Button confirmButton;            // Nút xác nhận

    void Start()
    {
        nameInputPanel.SetActive(true); // Hiện form ngay khi scene bắt đầu
        confirmButton.onClick.AddListener(OnSubmitName);
    }

    void OnSubmitName()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Tên không được để trống.");
            return;
        }

        // Gọi ScoreManager để lưu tên và điểm
        ScoreManager.instance.SaveHighScoreWithName(playerName);

        // Ẩn panel sau khi nhập
        nameInputPanel.SetActive(false);
    }
}
