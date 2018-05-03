using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : MonoBehaviour {

    public GameObject OpeningPanel;
    public GameObject PausePanel;
    public GameObject GamePanel;
    public Canvas Canvas;
	public bool Pause = false;

    // Use this for initialization
    void Start () {
        OpeningPanel.SetActive(true);
        PausePanel.SetActive(false);
        GamePanel.SetActive(false);
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
			Pause = false;
		}
    }

    public void PlayGame()
    {
        OpeningPanel.SetActive(false);
        PausePanel.SetActive(false);
        GamePanel.SetActive(true);
        Time.timeScale = 1f;
		Pause = false;
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
