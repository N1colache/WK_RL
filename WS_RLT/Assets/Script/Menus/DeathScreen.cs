using System;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{

    public LevelLoader levelLoaderRestart;
    public LevelLoader levelLoaderMenu;




    public void Restart()
    {
        Debug.Log("Restart");
        levelLoaderRestart.Transition();
    }
    
    public void Quit()
    {
        Debug.Log("Quit");
        levelLoaderMenu.Transition();
    }

}