using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    public static bool Paused = false;
    
    public GameObject pauseMenu;
    
    public LevelLoader levelLoader;


    private void Start()
    {
        Resume();
        levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (Paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }


    void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Paused = true;
    }


    void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Paused = false;
    }
    


    public void ResumeButton()
    {
        Resume();
    }

    public void QuitButton()
    {
        Debug.Log("Quit to menu");
        levelLoader.Transition();
    }

    public void SettingsButton()
    {
        Debug.Log("Open Settings");
    }
    
    
}
