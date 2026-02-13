using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Animator transition;
    
    public float transitionTime = 1.0f;
    
    public void PlayGame()
    {
        Debug.Log("Loaded Scene 1 Index");
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        
        SceneManager.LoadScene(1);
    }
    
    
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
    
}
