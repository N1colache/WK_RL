using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public LevelLoader levelLoader;
    private GameObject Script;
    

    public void PlayGame()
    {
        Debug.Log("Loaded Scene 1 Index");
        levelLoader.Transition();
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
}
