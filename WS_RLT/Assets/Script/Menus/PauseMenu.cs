using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    
    public static bool Paused = false;
    
    public GameObject pauseMenu;


    private void Start()
    {
        Resume();
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
    }

    public void SettingsButton()
    {
        Debug.Log("Open Settings");
    }
    
    
}
