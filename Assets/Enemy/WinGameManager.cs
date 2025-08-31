using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinGameManager : MonoBehaviour
{
    public GameObject winPanel;   // UI Panel
    public Button restartButton;  // زر لإعادة التشغيل

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0f; // يوقف الوقت
        if (winPanel != null)
            winPanel.SetActive(true);
        Debug.Log("✅ الفوز! اللاعب قتل العدو.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}