using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : MonoBehaviour {

    public GameObject OpeningPanel;
    public GameObject PausePanel;
    public GameObject GamePanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public Canvas Canvas;

    // Use this for initialization
    void Start() {
        OpeningPanel.SetActive(true);
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpeningPanel.SetActive(false);
            PausePanel.SetActive(true);
            GamePanel.SetActive(false);
            Time.timeScale = 0f;
        }
    }

    public void WinPanel()
    {
        winPanel.SetActive(true);
    }

    public void LosePanel()
    {
        losePanel.SetActive(true);
    }

    public void PlayGame()
    {
        OpeningPanel.SetActive(false);
        PausePanel.SetActive(false);
        GamePanel.SetActive(true);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Return()
    {
        OpeningPanel.SetActive(true);
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
    }


}
